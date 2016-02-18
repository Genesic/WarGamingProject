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
            $player->write("$cmd ".encode_json($data1)); # 設定player是位置1
            $rival->write("$cmd ".encode_json($data2)); # 設定rival是位置2
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
        my ($res, $skill, $combo) = $player->checkTouch($tempo);
        $player->sendCombo;
        # 如果combo中斷的話根據combo數施放skill
        if( $skill ){
            $rival->write("beSkilled ".encode_json([$skill]));
        }
    },

    # 防禦結果
    def_res => sub {
        my ($player, $cmd, $args) = @_;
        my $rival = $player->getRival;
        return if( !$rival );

        my $patch = $args->[0];
        my $hp = $player->patchHp($patch);
        $player->sendSync;
        if( $hp < 0 ){
            $player->write("end_game ".encode_json([Player::LOSE]));
            $rival->write("end_game ".encode_json([Player::WIN]));
            $player->gameOver;
            $rival->gameOver;
        }
    },
)
