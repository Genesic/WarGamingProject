package Player;

use Data::Dumper;
my $player_seq = 0;
sub new {
    my ($class, $hd) = @_;
    $player_seq++;
    my $self = {
        hd => $hd,
        rid => $player_seq,
    };
    bless $self, $class;

    $hd->push_read(line => qr/\r?\n|\0/, sub {
            my ($handle, $line) = @_;
            $self->processCmd($line);
            $hd->push_read(line => qr/\r?\n|\0/, callee);
        });

    return $self;
}

sub processCmd{
    my ($player, $line) = @_;
    my ($cmd, $args) = split / /,$line, 2;
}

1;
