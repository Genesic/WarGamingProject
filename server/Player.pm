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

    TOUCH_SUCCESS => 1,
    TOUCH_FAIL => 0,

    WIN => 2,
    LOSE => 1,
};

# SKILL
use constant {
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
    my %data = {
        pos => $pos,
        hp => 100,
        combo => 0,
    };
    $player->{game} = \%data;
}

sub getStartData {
    my ($player) = @_;
    my $playerGame = $player->{game};
    my $rival = getUser($player->{rival});
    my $rivalGame = $rival->{game};
    my %output = (
        pos => $playerGame->{pos},
        hp => $playerGame->{hp},
        rival_hp => $rivalGame->{hp},
    );

    return \%output;
}

sub checkReady {
    my ($player) = @_;
    $player->{checkReady} = 1;
    my $rival = getPlayer($player->{rival});
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
    return HIT_9;
}

sub checkTouch {
    my ($player, $touch) = @_;
    if( $touch == TOUCH_SUCCESS ){
        my $combo = $player->addCombo();
        return (1, 0, $combo);
    } elsif( $touch == TOUCH_FAIL ){
        my $combo = $player->clearCombo();
        my $skill = getSkillByCombo($combo);
        return (0, $skill, 0);
    }
}

sub sendCombo {
    my ($player) = @_;
    my $rival = getUser($player->{rival});
    my $data = [$player->{game}{pos}, $player->{game}{combo}];
    $player->write("combo ".encode_json($data));
    $rival->write("combo ".encode_json($data));
}

sub sendSync {
    my ($player) = @_;
    my $rival = getUser($player->{rival});
    my $data = [$player->{game}{pos}, $player->{game}{hp}];
    $player->write("combo ".encode_json($data));
    $rival->write("combo ".encode_json($data));
}

sub patchHp {
    my ($player, $patch) = @_;
    $player->{game}{hp} += $patch;
    return $player->{game}{hp}; 
}

1;
