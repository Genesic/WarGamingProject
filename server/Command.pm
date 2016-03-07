use strict;

use Data::dumper;
use NonPlayer;
use Room;

(
    # 選擇角色
    select_role => sub {
        my ($player, $cmd, $args) = @_;
        $player->{role} = $args->[0];
        $player->write("$cmd ".encode_json([1]));
    },

    # 選擇單人還是多人
    choose_mode => sub {
        my ($player, $cmd, $args) = @_;
        $player->{mode} = $args->[0];
        $player->write("$cmd ".encode_json([1]));
    },

    # 配對
    match => sub {
        my ($player, $cmd, $args) = @_;
        $player->{startMatch} = 1;
        if( $player->{mode} == Player::SINGLE_MODE ){
            # 單人模式
            my $room = Room->new($player->{rid}, Room::AI_PLAYER);
            $room->sendStartData($cmd);
        } elsif( $player->{mode} == Player::MULTIPLAY_MODE ){
            # 多人模式
            my ($res, $rival) = $player->match();
            if( $res ){                
                my $room = Room->new($player->{rid}, $rival->{rid});
                $room->sendStartData($cmd);
            } else {
                my $data = { res => 0, reason => "wait" };
                $player->write("$cmd ".encode_json($data));
            }
        }
    },

    # client準備完成可以開始遊戲
    ready => sub {
        my ($player, $cmd, $args) = @_;
        $player->{room}->checkReady($player);
    },

    # 每一拍傳送過來的結果
    tempo => sub {
        my ($player, $cmd, $args) = @_;
        my $room = $player->{room};
        return if( !$room );

        my $tempo = $args->[0];
        $room->checkTempo($player, $tempo);
    },

    # 使用技能(會先放到自己的skill_queue中，可以被執行時會再丟be_skill過來通知server傳送給兩邊執行)
    skill => sub {
        my ($player, $cmd, $args) = @_;
        my $room = $player->{room};
        return if( !$room );
        my ($res, $skill) = $room->useSkill($player, $args->[0], $args->[1]);
        if( $res ){
            $room->sendSync($player, "mp");
            $room->sendSkillQueue;
        }
    },

    # 被使用技能開始執行
    be_skill => sub{
        my ($player, $cmd, $args) = @_;
        my $room = $player->{room};
        my ($res, $rid, $skill) = $room->getBeSkill($player);
        if( $res ) {
            $room->sendSkillQueue;
            my $caster = $room->getPlayer($rid); 
            my $victim = $room->getRival($caster);
            $caster->write("use_skill ".encode_json([$skill])) if( $caster );
            $victim->write("be_skill ".encode_json([$skill])) if( $victim );
        }
    },

    # 防禦結果
    def_res => sub {
        my ($player, $cmd, $args) = @_;
        my $room = $player->{room};
        return if( !$room );

        my ($id, $res) = @$args;
        my $patch = ($res)? 0 : -5;
        my $hp = $room->patchHp($player, $patch);
        $room->sendSync($player, "hp");
        $room->sendDefShow($player, $id, $res);
        $room->destroy if( $room->isGameOver ); # end_game在checkGameOver裡面送
    },
)
