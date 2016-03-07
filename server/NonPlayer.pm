package NonPlayer;

use strict;
use AE; use callee;
use POSIX qw(strftime);
use Data::Dumper;
use JSON;
use Config;

use Room;

our %command;
sub new {
    my ($class, $rival_rid) = @_;
    my $role = int (rand 3) + 1;
    my $self = {
        rid => Room::AI_PLAYER,
        role => $role,
    };
    bless $self, $class;

    return $self;
}
sub startSkillQueueChecker {
    my ($nonPlayer) = @_;
    $nonPlayer->{skill_timer} = AE::timer 1, 1, sub{
        my $room = $nonPlayer->{room};
        # 被施放技能的條件
        # 1. skill_queue裡面有東西
        # 2. 沒有被施放技能(沒有執行be_skill)
        # 3. 沒有施放技能中
        my @count = ( $nonPlayer->{casting} )? keys %{$nonPlayer->{casting}} : ();
        if(scalar @{$room->{skill_queue}} > 0 && !$nonPlayer->{be_skill_timer} && scalar @count == 0){
            my $info = $room->{skill_queue}[0];
            my ($casterRid, $skillId) = @$info;
            if( $casterRid != $nonPlayer->{rid} ){
                my ($res, $casterRid, $skillId) = $room->getBeSkill($nonPlayer);
                if( $res ){
                    $room->sendSkillQueue;
                    my $caster = $room->getPlayer($casterRid); 
                    my $victim = $room->getRival($caster);
                    $caster->write("use_skill ".encode_json([$skillId])) if( $caster );
                    $victim->write("be_skill ".encode_json([$skillId])) if( $victim );
                } 
            }
        }
    };
}

sub startTempoChecker {
    my ($nonPlayer) = @_;
    $nonPlayer->{tempo_timer} = AE::timer 2, 2, sub{
        my $room = $nonPlayer->{room};
        $room->checkTempo($nonPlayer, Room::TOUCH_SUCCESS);
        my $mp = $room->getPlayerGameData($nonPlayer)->{"mp"};
        if( $mp > 4 && ref $room->{skill_queue} eq 'ARRAY' && scalar @{$room->{skill_queue}} < 4){
            my ($res, $skill) = $room->useSkill($nonPlayer, 1, 2);
            if( $res ){
                $room->sendSync($nonPlayer, "mp");
                $room->sendSkillQueue;
            }
        }
    };
}

sub write{
    my ($nonPlayer, $line) = @_;
    my ($cmd, $json) = split / /, $line, 2;
    my $args = ($json)? decode_json($json) : [];

    if( $cmd eq 'start_game' ){
        $nonPlayer->startSkillQueueChecker;
        $nonPlayer->startTempoChecker;
    } elsif( $cmd eq 'sync' ){
        for my $key (qw(hp combo mp) ){
            $nonPlayer->{game}{$key} = $args->{$key} if( $args->{$key} );
        }
    } elsif ( $cmd eq 'be_skill' ){
        my %skillCount = (
            1 => 9,
            2 => 16,
        );
        my $skill_id = $args->[0];
        $nonPlayer->{be_skill_id} = 0;
        $nonPlayer->{be_skill_timer} = AE::timer 1, 1, sub{
            if( $nonPlayer->{be_skill_id} < $skillCount{$skill_id} ){
                my $rand = int (rand 100 );
                my $res;
                if( $rand > 50 ){
                    $res = 0; # 防禦失敗
                    my $room = $nonPlayer->{room};
                    my $hp = $room->patchHp($nonPlayer, -5);
                    my $res = $room->sendSync($nonPlayer, "hp");
                    $room->destroy if( $room->isGameOver ); # end_game在checkGameOver裡面送
                } else {
                    $res = 1; # 防禦成功
                }
                $nonPlayer->{room}->sendDefShow($nonPlayer, $nonPlayer->{be_skill_id}, $res) if( $nonPlayer->{room} );
            } else {
                delete $nonPlayer->{be_skill_timer};
            }
            $nonPlayer->{be_skill_id}++;
        }; 
    } elsif( $cmd eq 'use_skill' ){
        #  記錄施放的技能
        my %atkIds = map { $_ => 1 } (0..8);
        $nonPlayer->{casting} = \%atkIds;
    } elsif( $cmd eq 'atk_show' ){
        delete $nonPlayer->{casting}{$args->[0]};
    }
}

sub isPlayer {
    return 0;
}

sub endGame {
    my ($nonPlayer) = @_;
    delete $nonPlayer->{skill_timer};
    delete $nonPlayer->{tempo_timer};
    delete $nonPlayer->{be_skill_timer};
}
1;
