package Player;

use strict;
use AE; use callee;
use POSIX qw(strftime);
use Data::Dumper;
use JSON;
use Config;

my $player_seq = 0;
our %command;
our %playerList;

use constant {
    NOLOG => 1,
    PLAYER1 => 1,
    PLAYER2 => 2,
    AI_PLAYER => -1,

    SINGLE_MODE => 1,
    MULTIPLAY_MODE => 2,

    TOUCH_SP => 2,
    TOUCH_SUCCESS => 1,
    TOUCH_FAIL => 0,

    WIN => 2,
    LOSE => 1,
};

sub addUser {
    my ($player) = @_;
    $playerList{$player->{rid}} = $player;
}

sub getUser {
    my ($rid) = @_;
    return $playerList{$rid};
}

sub getRival {
    my ($player) = @_;
    return undef if( !$player->{rival} );
    if( $player->{rival} > 0 ){
        my $rival = getUser($player->{rival});
        return $rival;
    } else {
        return $player->{nonPlayer};
    }
}

sub new {
    my ($class, $hd) = @_;
    $player_seq++;
    my $self = {
        hd => $hd,
        rid => $player_seq,
    };
    bless $self, $class;

    $self->addUser;
    $self->write("login");
    $hd->push_read(line => qr/\r?\n|\0/, sub {
            my ($handle, $line) = @_;
            $self->processCmd($line);
            $hd->push_read(line => qr/\r?\n|\0/, callee);
        });
    $self->ping;
    return $self;
}

sub ping {
    my ($player) = @_;

    $player->write("ping", NOLOG);
    $player->{pingTimer} = AE::timer 10, 0, sub{
        $player->destroy;
    };
}

sub pong {
    my ($player) = @_;
    delete $player->{pingTimer};
    $player->{pongTimer} = AE::timer 10, 0, sub{
        undef $player->{pongTimer};
        $player->ping;
    };
}

sub destroy {
    my ($player) = @_;
    $player->dolog("", "Destroyed");
    delete $playerList{$player->{rid}};
    for (keys %$player ){
        delete $player->{$_};
    }
}

sub load_cmd {
    my ($player) = @_;
    my %tmp = do 'Command.pm';
    if( %tmp ){
        %command = %tmp;
    } else {
        $player->dolog( "DEBUG", "load_cmd fail");
    }
}

sub write{
    my ($player, $line, $nolog) = @_;
    my $hd = $player->{hd};
    $player->dolog("Send", $line) if( !$nolog );
    $player->{hd}->push_write("$line\n") if( $hd );
}

sub dolog {
    my ($player, $prefix, $line) = @_;
    my $time = sprintf('%.3f', AE::now);
    my $stamp = strftime('%m/%d %T', localtime int $time) . substr($time, index($time, '.'), 4);
    print "$stamp $prefix(Player: $player->{rid}) : $line\n";
}

sub processCmd{
    my ($player, $line) = @_;
    my ($cmd, $json) = split / /,$line, 2;
    if( $cmd eq 'pong' ){
        $player->pong;
    } else {
        $player->dolog("In", $line);
        my $args = ($json)? decode_json($json) : [];
        $player->load_cmd();
        if( defined $command{$cmd} ){
            $command{$cmd}->($player, $cmd, $args);
        } else {
            $player->dolog("Undef", $line);
        }
    }
}

sub match {
    my ($player) = @_;

    my $rival;
    for my $rid (keys %playerList){
        my $obj = $playerList{$rid};
        next if( $rid == $player->{rid} );
        next if( !$obj->{startMatch} );
        next if( $obj->{rival} );
        $rival = $obj;
        last;
    }

    if( $rival ){
        return (1, $rival);
    } else {
        return (0);
    }
}

sub setStartData {
    my ($player, $rival, $pos) = @_;
    # 取消配對狀態
    delete $player->{startMatch}; 

    $player->{rival} = $rival->{rid};
    my %data = (
        pos => $pos,
        hp => 100,
        mp => 0,
        combo => 0,
        skill_queue => [],
    );
    $player->{game} = \%data;

    if( $player->{rival} < 0 ){
        $player->{nonPlayer} = $rival;
    }
}

sub getStartData {
    my ($player) = @_;
    my $playerGame = $player->{game};
    my $rival = $player->getRival;
    return {} if( !$rival );

    my $rivalGame = $rival->{game};
    my %output = (
        res => 1,
        role => $player->{role},
        rival_role => $rival->{role},
    );

    return \%output;
}

sub checkReady {
    my ($player) = @_;
    $player->{checkReady} = 1;
    my $rival = $player->getRival;
    return (1, $rival) if( $rival && $rival->{checkReady} );
    return (0);
}

sub addCombo {
    my ($player, $patch) = @_;
    $player->{game}{combo} += $patch;
    return $player->{game}{combo};
}

sub clearCombo {
    my ($player) = @_;
    my $combo = $player->{game}{combo};
    $player->{game}{combo} = 0;
    return $combo;
}

sub useSkill {
    my ($player, $skill, $cost) = @_;
    if( $player->{game}{mp} >= $cost ){
        $player->patchMp(-$cost);
        $player->pushSkillQueue($skill);
        return (1, $skill);
    } else {
        return (0, 0);
    }
}

sub checkTempo {
    my ($player, $touch) = @_;
    if( $touch == TOUCH_SP ){
        my $combo = $player->addCombo(1);
        $player->updateMpByCombo($combo, 5);
    } elsif( $touch == TOUCH_SUCCESS ){
        my $combo = $player->addCombo(1);
        $player->updateMpByCombo($combo, 1);
    } elsif($touch == TOUCH_FAIL ) {
        $player->clearCombo;
    }
}

sub pushSkillQueue {
    my ($player, $skill) = @_;
    push @{$player->{game}{skill_queue}}, $skill;
}

sub getBeSkill{
    my ($player) = @_;
    my $rival = $player->getRival;
    return if( !$rival );

    my $skill = shift @{$rival->{game}{skill_queue}};
    if( $skill ){
        return (1, $skill, $rival);
    }
    return (0);
}

sub updateMpByCombo {
    my ($player, $combo, $sp) = @_;

    if( $combo < 20 ){
        $player->patchMp(1*$sp);
    } elsif( $combo < 40 ){
        $player->patchMp(2*$sp);
    } elsif( $combo < 60 ){
        $player->patchMp(3*$sp);
    } elsif( $combo < 80 ){
        $player->patchMp(4*$sp);
    } else {
        $player->patchMp(5*$sp);
    }
}

sub sendSync {
    my ($player, $key) = @_;
    my $rival = $player->getRival;
    return if( !$rival );
    my $data = { $key => $player->{game}{$key} };
    my $rivalData = { "rival_".$key => $player->{game}{$key} };
    $player->write("sync ".encode_json($data));
    $rival->write("sync ".encode_json($rivalData));
}

sub patchHp {
    my ($player, $patch) = @_;
    $player->{game}{hp} += $patch;
    return $player->{game}{hp}; 
}

sub patchMp {
    my ($player, $patch) = @_;
    $player->{game}{mp} += $patch;
    return $player->{game}{mp}; 
}

sub gameOver {
    my ($player) = @_;
    delete $player->{rival};
}

sub isPlayer {
    return 1;
}

1;
