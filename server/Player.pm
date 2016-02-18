package Player;

use POSIX qw(strftime);
use Data::Dumper;
use JSON;

my $player_seq = 0;
our %cmd;
our %playerList;

use constant {
    PLAYER1 => 1,
    PLAYER2 => 2,
}

sub addUser {
    my ($player) = @_;
    $playerList{$player->{rid}} = $player;
}

sub getUser {
    my ($rid) = @_;
    return $playerList{$rid};
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
    $hd->push_read(line => qr/\r?\n|\0/, sub {
            my ($handle, $line) = @_;
            $self->processCmd($line);
            $hd->push_read(line => qr/\r?\n|\0/, callee);
        });

    return $self;
}

sub load_cmd {
    my ($player) = @_;
    my %tmp = do 'Command.pm';
    if( %tmp ){
        %cmd = %tmp;
    } else {
        $player->dolog($player, "DEBUG", "load_cmd fail");
    }
}

sub write{
    my ($player, $line, $nolog) = @_;
    $player->dolog("Send", $line) if( !$nolog );
    $player->{hd}->push_write("$line\n");
}

sub dolog {
    my ($player, $prefix, $line) = @_;
    my $time = sprintf('%.3f', AE::now);
    my $stamp = strftime('%m/%d %T', localtime int $time) . substr($time, index($time, '.'), 4);
    print "$stamp $prefix(Player: $player->{rid}) : $line\n";
}

sub processCmd{
    my ($player, $line) = @_;
    $player->dolog("In", $line);
    my ($cmd, $json) = split / /,$line, 2;
    my $args = decode_json($json);
    load_cmd();
    $cmd{$cmd}->($player, $cmd, $args);
}

sub match {
    my ($player) = @_;

    for my $rid (keys %playerList){
        my $obj = $playerList{$rid};
        next if( !$obj->{startMatch} );
        $rival = $obj;
        last;
    }

    if( $rival ){
        return (1, $rival);
    } else {
        return (0);
    }
}

1;
