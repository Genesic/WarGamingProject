package NonPlayer;

use strict;
use AE; use callee;
use POSIX qw(strftime);
use Data::Dumper;
use JSON;
use Config;

use Player;

our %command;
sub new {
    my ($class, $rival_rid) = @_;
    my $role = int (rand 3) + 1;
    my $self = {
        rid => Player::AI_PLAYER,
        role => $role,
        checkReady => 1,
        rival => $rival_rid,
    };
    bless $self, $class;

    return $self;
}

sub getRival {
    my ($nonplayer) = @_;
    my $rival_rid = $nonplayer->{rival};
    return Player::getUser($rival_rid);
}

sub write{
    my ($nonPlayer, $line) = @_;
    my ($cmd, $json) = split / /, $line, 2;
    my $args = ($json)? decode_json($json) : [];

    if( $cmd eq 'sync' ){
        for my $key (qw(hp combo mp) ){
            $nonPlayer->{game}{$key} = $args->{$key} if( $args->{$key} );
        }

        if( $args->{rival_skill_queue} ){
            $nonPlayer->{rival_skill_queue} = $args->{rival_skill_queue}; 
            $nonPlayer->{skill_queue_timer} = AE::timer 1, 5, sub{
                if( @{$nonPlayer->{rival_skill_queue}} > 0 ){
                    my $skill_id = shift @{$nonPlayer->{rival_skill_queue}};
                    my ($res, $skill, $rival) = $nonPlayer->getBeSkill();
                    if( $res ) {
                        $rival->sendSync("skill_queue");
                        $rival->write("use_skill ".encode_json([$skill]));
                    }
                } else {
                    delete $nonPlayer->{skill_queue_timer};
                }
            };
        }
    }
}

sub setStartData {
    my ($nonPlayer, $rival, $pos) = @_;

    $nonPlayer->{rival} = $rival->{rid};
    my %data = (
        pos => $pos,
        hp => 100,
        mp => 0,
        combo => 0,
        skill_queue => [],
    );
    $nonPlayer->{game} = \%data;
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
    if( $touch == Player::TOUCH_SP ){
        my $combo = $player->addCombo(1);
        $player->updateMpByCombo($combo, 5);
    } elsif( $touch == Player::TOUCH_SUCCESS ){
        my $combo = $player->addCombo(1);
        $player->updateMpByCombo($combo, 1);
    } elsif($touch == Player::TOUCH_FAIL ) {
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
    return 0;
}
1;
