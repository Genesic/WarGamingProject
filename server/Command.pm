use strict;

use Data::dumper;

(
    # 選擇角色
    select_role => sub {
        my ($player, $cmd, $args) = @_;
        $player->{role} = $args->[0];
        $player->write("$cmd ".encode_json([1]));
    },

    # 配對
    match => sub {
        my ($player, $cmd, $args) = @_;
        $player->{startMatch} = 1;
        my ($res, $rival) = $player->match();
        if( $res ){
            $player->setStartData($rival, Player::PLAYER1);
            $rival->setStartData($player, Player::PLAYER2);
            my $data1 = $player->getStartData();
            my $data2 = $rival->getStartData();
            $player->write("$cmd ".encode_json($data1));
            $rival->write("$cmd ".encode_json($data2));
        } else {
            my $data = { res => 0, reason => "wait" };
            $player->write("$cmd ".encode_json($data));
        }
    },

    # client準備完成可以開始遊戲
    ready => sub {
        my ($player, $cmd, $args) = @_;
        my ($res, $rival) = $player->checkReady();
        if( $res ){
            $player->write("start_game ".encode_json([1]));
            $rival->write("start_game ".encode_json([1]));
        }
    },

    # 每一拍傳送過來的結果
    tempo => sub {
        my ($player, $cmd, $args) = @_;
        my $rival = $player->getRival;
        return if( !$rival );

        my $tempo = $args->[0];
        $player->checkTempo($tempo);
        $player->sendSync("combo");
        $player->sendSync("mp");
    },

    # 使用技能(會先放到對方client的be_skill_queue中，可以被執行時會再丟be_skill過來通知server傳送給兩邊執行)
    skill => sub {
        my ($player, $cmd, $args) = @_;
        my ($res, $skill) = $player->useSkill($args->[0], $args->[1]);
        if( $res ){
            $player->sendSync("mp");
            $player->sendSync("skill_queue");
        }
    },

    # 被使用技能開始執行
    be_skill => sub{
        my ($player, $cmd, $args) = @_;
        my ($res, $skill, $rival) = $player->getBeSkill();
        if( $res ) {
            $rival->sendSync("skill_queue");
            $player->write("be_skill ".encode_json([$skill]));
            $rival->write("use_skill ".encode_json([$skill]));
        }
    },

    # 防禦結果
    def_res => sub {
        my ($player, $cmd, $args) = @_;
        my $rival = $player->getRival;
        return if( !$rival );

        my $patch = ($args->[0])? 0 : -5;
        my $hp = $player->patchHp($patch);
        $player->sendSync("hp");
        if( $hp < 0 ){
            $player->write("end_game ".encode_json([Player::LOSE]));
            $rival->write("end_game ".encode_json([Player::WIN]));
            $player->gameOver;
            $rival->gameOver;
        }
    },
)
