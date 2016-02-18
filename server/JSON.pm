package JSON;

use strict;
use JSON ();
use Encode ();

use base qw(Exporter);
our @EXPORT_OK = qw(encode_json decode_json decode_json_safe);

sub encode_json {
    return JSON::encode_json(decode_utf8($_[0]));
}

sub decode_json_safe { eval { local $SIG{__DIE__}; decode_json($_[0]); } }
sub decode_json {
    my $str = $_[0];
    $str =~ s/^[^[{]+//;
    my $tmp = JSON::decode_json($str);
    return encode_utf8($tmp);
}

sub encode_utf8 {
    if( ref($_[0]) eq 'ARRAY' ) {
	return [ map { encode_utf8($_) } @{$_[0]} ];
    }
    elsif( ref($_[0]) eq 'HASH' ) {
	return { map { encode_utf8($_) } %{$_[0]} };
    }
    else {
	my $data = shift;
	#Encode::_utf8_off($data);
	$data = Encode::encode('utf8', $data);
	return $data;
    }
}

sub decode_utf8 {
    if( ref($_[0]) eq 'ARRAY' ) {
	return [ map { decode_utf8($_) } @{$_[0]} ];
    }
    elsif( ref($_[0]) eq 'HASH' ) {
	return { map { decode_utf8($_) } %{$_[0]} };
    }
    else {
	my $data = shift;
	#Encode::_utf8_on($data);
	$data = Encode::decode('utf8', $data);
	return $data;
    }
}

1;
