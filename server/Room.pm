package Room;

use strict;
use AE; use callee;
use POSIX qw(strftime);
use Data::Dumper;
use JSON;
use Config;

use Player;

use constant {
    AI_PLAYER => -1,

    SINGLE_MODE => 1,
    MULTIPLAY_MODE => 2,

    WIN => 1,
    LOSE => 2,

    TOUCH_SP => 2,
    TOUCH_SUCCESS => 1,
    TOUCH_FAIL => 0,
};

sub getInitGameData {
    my ($room) = @_;
    my %data = (
        hp => 100,
        mp => 0,
        combo => 0,
    );

    return \%data;
}

sub new {
    my ($class, $player1_rid, $player2_rid) = @_;    
    my $player1 = ($player1_rid > 0)? Player::getUser($player1_rid) : NonPlayer->new() ;    
    my $player2 = ($player2_rid > 0)? Player::getUser($player2_rid) : NonPlayer->new() ;
    my $mode = ($player1_rid < 0 || $player2_rid < 0)? SINGLE_MODE : MULTIPLAY_MODE;
    my $data1 = getInitGameData();
    my $data2 = getInitGameData();
    my %gameData = (
        $player1->{rid} => $data1,
        $player2->{rid} => $data2,
    );
    my $self = {
        mode => $mode,
        player => [$player1, $player2],
        gameData => \%gameData,
        skill_queue => [],
        ready => {},
    };
    bless $self, $class;

    $player1->{room} = $self;
    $player2->{room} = $self;

    # 所有AI都設為ready
    for my $player ( @{$self->{player}} ){
        $self->{ready}{$player->{rid}} = 1 if( !$player->isPlayer );
    }

    return $self;
}

sub destroy {
    my ($room) = @_;
    for my $player (@{$room->{player}}){
        delete $player->{room};
    }
    for (keys %$room){
        delete $room->{$_}
    }
}

sub getPlayerGameData {
    my ($room, $player) = @_;
    return $room->{gameData}{$player->{rid}};
}

sub getRival {
    my ($room, $player) = @_;
    for my $user (@{$room->{player}}){
        return $user if( $player->{rid} != $user->{rid} );
    }
    return;
}

sub getPlayer{
    my ($room, $rid) = @_;
    for my $player (@{$room->{player}}){
        return $player if( $player->{rid} == $rid );
    }

    return;
}

sub sendStartData {
    my ($room, $cmd) = @_;
    for my $player (@{$room->{player}}){
        if( $player->isPlayer ){
            my $rival = $room->getRival($player);
            my %output = (
                res => 1,
                role => $player->{role},
                rival_role => $rival->{role},
            );
            $player->write("$cmd ".encode_json(\%output));
        }
    }
}

sub checkReady {
    my ($room, $player) = @_;
    $room->{ready}{$player->{rid}} = 1;
    for my $user (@{$room->{player}}){
        return if( !$room->{ready}{$user->{rid}} );
    }

    # 所有玩家都已經ready
    for my $user (@{$room->{player}}){
        $user->write("start_game ".encode_json([1]));
    }
}

sub addCombo {
    my ($room, $player, $patch) = @_;
    my $gameData = $room->{gameData}{$player->{rid}};
    $gameData->{combo} += $patch;
    return $gameData->{combo};
}

sub clearCombo {
    my ($room, $player) = @_;
    my $gameData = $room->{gameData}{$player->{rid}};
    my $combo = $gameData->{combo};
    $gameData->{combo} = 0;
    return $combo;
}

sub useSkill {
    my ($room, $player, $skill, $cost) = @_;
    my $gameData = $room->getPlayerGameData($player);
    if( $gameData->{mp} >= $cost ){
        $room->patchMp($player, -$cost);
        $room->pushSkillQueue($player, $skill);
        return (1, $skill);
    } else {
        return (0, 0);
    }
}

sub checkTempo {
    my ($room, $player, $touch) = @_;
    if( $touch == TOUCH_SP ){
        my $combo = $room->addCombo($player, 1);
        $room->updateMpByCombo($player,$combo, 5);
    } elsif( $touch == TOUCH_SUCCESS ){
        my $combo = $room->addCombo($player, 1);
        $room->updateMpByCombo($player, $combo, 1);
    } elsif($touch == TOUCH_FAIL ) {
        $room->clearCombo($player);
    }

    $room->sendSync($player, "combo");
    $room->sendSync($player, "mp");
}

sub pushSkillQueue {
    my ($room, $player, $skill) = @_;
    push @{$room->{skill_queue}}, [$player->{rid}, $skill];
}

sub getBeSkill{
    my ($room, $player) = @_;
    my $room = $player->{room};
    return if( !$room );

    my $skillData = shift @{$room->{skill_queue}};
    my ($rid, $skill) = @$skillData;
    if( $skill ){
        return (1, $rid, $skill);
    }
    return (0);
}

sub updateMpByCombo {
    my ($room, $player, $combo, $sp) = @_;

    if( $combo < 20 ){
        $room->patchMp($player, 1*$sp);
    } elsif( $combo < 40 ){
        $room->patchMp($player, 2*$sp);
    } elsif( $combo < 60 ){
        $room->patchMp($player, 3*$sp);
    } elsif( $combo < 80 ){
        $room->patchMp($player, 4*$sp);
    } else {
        $room->patchMp($player, 5*$sp);
    }
}

sub sendSync {
    my ($room, $player, $key) = @_;
    my $rival = $room->getRival($player);
    return 0 if( !$rival );

    my $value = $room->getPlayerGameData($player)->{$key};
    my $data = { $key => $value };
    my $rivalData = { "rival_".$key => $value };
    $player->write("sync ".encode_json($data));
    $rival->write("sync ".encode_json($rivalData));
    return 1;
}

use constant {
    SKILL_SELF => 1,
    SKILL_RIVAL => 2,
};

sub sendSkillQueue {
    my ($room) = @_;

    for my $player (@{$room->{player}}){
        my @output;
        for my $skill (@{$room->{skill_queue}}){
            my ($rid, $skill_id) = @$skill;
            my $caster = ($rid == $player->{rid})? SKILL_SELF : SKILL_RIVAL;
            push @output, [$caster, $skill_id];
        }
        $player->write("skill_queue ".encode_json(\@output));
    }
}

sub sendDefShow {
    my ($room, $defender, $def_id, $def_res) = @_;
    my $attacker = $room->getRival($defender);
    $attacker->write("atk_show ".encode_json([$def_id, $def_res]));
    $defender->write("def_show ".encode_json([$def_id, $def_res]));
}

sub patchHp {
    my ($room, $player, $patch) = @_;
    my $gameData = $room->getPlayerGameData($player);
    $gameData->{hp} += $patch;
    return $gameData->{hp};
}

sub patchMp {
    my ($room, $player, $patch) = @_;
    my $gameData = $room->getPlayerGameData($player);
    $gameData->{mp} += $patch;
    return $gameData->{mp}; 
}

sub isGameOver {
    my ($room) = @_;
    for my $player (@{$room->{player}}){
        my $gameData = $room->getPlayerGameData($player);
        if( $gameData->{hp} <= 0 ){
            my $rival = $room->getRival($player);
            $rival->endGame;
            $player->endGame;
            $rival->write("end_game ".encode_json([WIN]));
            $player->write("end_game ".encode_json([LOSE]));
            return 1;
        }
    }

    return 0;
}
1;
