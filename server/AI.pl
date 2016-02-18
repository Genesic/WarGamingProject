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
        $client->write("select_role [1]");
    };
}

sub handleRead{
    my($handle, $player) = @_;

    $handle->push_read(line => qr/\r?\n|\0/, sub {
            processCmd(@_);
            $handle->push_read(line => qr/\r?\n|\0/, callee);
        });
}

main();
1;

package Client;
use AE; use callee;
use POSIX qw(strftime);

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
    my ($cmd, $args) = split / /, $line, 2;
    print "cmd:$cmd args:$args\n";
}

1;
