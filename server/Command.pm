use strict;

use Data::dumper;

(
    # 選擇角色
    select_role => sub {
        my ($player, $cmd, $args) = @_;
        $player->{role} = $args->[0];
    },

    # 配對
    match => sub {
        my ($player, $cmd, $args) = @_;
        $player->{startMatch} = 1;
        my ($res, $rival) = $player->match();
        if( $res ){
            $player->write("match ".encode_json([1, Player::PLAYER1])); # 設定player是位置1
            $rival->write("match ".encode_json([1, Player::PLAYER2])); # 設定rival是位置2
        } else {
            $player->write("match ".encode_json([0, "wait"]));
        }
    },

    # client準備完成可以開始遊戲
    ready => sub {
        my ($player, $cmd, $args
    },    
)
