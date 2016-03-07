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

    SINGLE_MODE => 1,
    MULTIPLAY_MODE => 2,
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
        # 取消配對狀態
        delete $player->{startMatch}; 
        delete $rival->{startMatch};
        return (1, $rival);
    } else {
        return (0);
    }
}

sub isPlayer {
    return 1;
}

sub endGame {
}
1;
