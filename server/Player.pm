package Player;

use strict;
use AE; use callee;
use POSIX qw(strftime);
use Data::Dumper;
use JSON;

my $player_seq = 0;
our %command;
our %playerList;

use constant {
    NOLOG => 1,
    PLAYER1 => 1,
    PLAYER2 => 2,

    TOUCH_SUCCESS => 1,
    TOUCH_FAIL => 0,

    WIN => 2,
    LOSE => 1,
};

# SKILL
use constant {
    NONE => 0,
    HIT_9 => 1,
    HIT_18 => 2,
    HIT_27 => 3,
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
    my $rival = getUser($player->{rival});
    return $rival;
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
        $player->dolog($player, "DEBUG", "load_cmd fail");
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
        load_cmd();
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
        combo => 0,
    );
    $player->{game} = \%data;
}

sub getStartData {
    my ($player) = @_;
    my $playerGame = $player->{game};
    my $rival = $player->getRival;
    return {} if( !$rival );

    my $rivalGame = $rival->{game};
    my %output = (
        res => 1,
        pos => $playerGame->{pos},
        role => $player->{role},
        rival_role => $rival->{role},
        hp => $playerGame->{hp},
        rival_hp => $rivalGame->{hp},
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

sub getSkillByCombo {
    my ($combo) = @_;
    return HIT_18 if( $combo > 10 );
    return HIT_9 if( $combo > 5 );
    return NONE;
}

sub checkTempo {
    my ($player, $touch) = @_;
    if( $touch == TOUCH_SUCCESS ){
        my $combo = $player->addCombo(1);
        return (1, 0, $combo);
    } elsif( $touch == TOUCH_FAIL ){
        my $combo = $player->clearCombo();
        my $skill = getSkillByCombo($combo);
        return (0, $skill, 0);
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

sub gameOver {
    my ($player) = @_;
    delete $player->{rival};
}

1;
