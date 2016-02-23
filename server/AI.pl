#!/usr/bin/perl
use strict;
use warnings;

use AnyEvent;
use AnyEvent::Socket;
use AnyEvent::Handle;
use AE; use callee;
use feature qw(say state switch);
use JSON qw(encode_json decode_json);
use Getopt::Std;
use POSIX qw(strftime);
use Data::Dumper;

use Player;

my $cv = AE::cv;
sub main {
    my(%opt, @port) = ();
    getopts("h:p:",\%opt);
    start_connect($opt{h},$opt{p});
    $cv->recv();
}

sub start_connect{
    my ($host, $port) = @_;
    $host ||= "127.0.0.1";
    print "connect to server $host:$port\n";
    tcp_connect $host, $port, sub {
        my($fh, $host, $port) = @_;
        my $handle; $handle = AnyEvent::Handle->new
        ( fh => $fh,
            on_error => sub {         # including on_eof
                my $timer; $timer = AE::timer 0, 0, sub {
                    undef $handle;
                    undef $timer;
                }
            }
        );

        my $client = new Client($handle);
        $client->write("login [1]");
    };
}

main();
1;

package Client;
use AE; use callee;
use POSIX qw(strftime);
use JSON qw(encode_json decode_json);

sub new {
    my ($class, $hd) = @_;

    my $self = {
        hd => $hd,
    }; bless $self, $class;

    $hd->push_read(line => qr/\r?\n|\0/, sub {
            $self->processCmd(@_);
            $hd->push_read(line => qr/\r?\n|\0/, callee);
        });

    return $self;
}

sub dolog {
    my ($player, $prefix, $line) = @_;
    my $time = sprintf('%.3f', AE::now);
    my $stamp = strftime('%m/%d %T', localtime int $time) . substr($time, index($time, '.'), 4);
    print "$stamp $prefix:$line\n";
}

sub write{
    my ($player, $line, $nolog) = @_;
    $player->dolog("Send", $line) if( !$nolog );
    $player->{hd}->push_write("$line\n");
}

sub processCmd{
    my ($client, $hd, $line) = @_;
    $client->dolog("In",$line);
    my ($cmd, $json) = split / /, $line, 2;
    my $args = ($json)? decode_json($json) : [];

    if( $cmd eq 'ping' ){
        $client->write("pong");
    } if( $cmd eq 'login' ){
        $client->write("select_role [1]");
    } elsif( $cmd eq 'select_role' ){
        $client->write("match");
    } elsif( $cmd eq 'match' ){
        $client->write("ready") if( $args->{res} );
    } elsif( $cmd eq 'start_game' ){
        $client->{tempo_timer_count} = 0;
        $client->{tempo_timer} = AE::timer 1, 1, sub{
            $client->{tempo_timer_count}++;
            if( $client->{tempo_timer_count} > 13 ){
                $client->write("tempo [0]");
                delete $client->{tempo_timer};
            } else {
                $client->write("tempo [1]");
            }
        };
    } elsif( $cmd eq 'be_skill' ){
        $client->{skill_timer_count} = 0;
        $client->{skill_timer} = AE::timer 1, 1, sub{
            $client->{skill_timer_count}++;
            $client->write("def_res [0]");
            delete $client->{skill_timer} if( $client->{skill_timer_count} >=9 );
        };
    }
}

1;
