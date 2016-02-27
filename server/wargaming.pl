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
use Data::Dumper;
use POSIX qw(strftime);

use Carp;

use Player;

my $cv = AE::cv;
sub main {
    my(%opt, @port) = ();
    getopts("Ep:",\%opt);
    $SIG{ __DIE__ } = sub { Carp::confess( @_ ) };
    if( $opt{E} ){
        my $file = 'Command.pm';
        do $file;
        print "do $file :". ($@ || 'ok').$/;
        exit 0;
    } else {
        start_server($opt{p});
    }
    $cv->recv();
}

sub start_server{
    my ($port) = @_;

    print "server start(port $port)\n";
    tcp_server undef, $port, sub {
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

        $handle->push_read(line => qr/\r?\n|\0/, sub {
                my($handle, $line) = @_;
                my ($cmd) = split/ /, $line, 2;
                dolog("In",$line);
                if( $cmd eq 'login' ){
                    my $player = new Player($handle);
                }
            });
    };
}

sub dolog {
    my ($prefix, $line) = @_;
    my $time = sprintf('%.3f', AE::now);
    my $stamp = strftime('%m/%d %T', localtime int $time) . substr($time, index($time, '.'), 4);
    print "$stamp $prefix:$line\n";
}

main();
1;
