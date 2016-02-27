package Config;
use strict;

# SKILL
use constant {
    NONE => 0,
    SKILL_1 => 1,
    SKILL_2 => 2,
    SKILL_3 => 3,
};

my %skill = (
    (SKILL_1) => {
        name => "九頭龍閃",
        mp => 30,
    },
    (SKILL_2) => {
        name => "降龍十八掌",
        mp => 35,
    },
    (SKILL_2) => {
        name => "北斗百裂拳",
        mp => 40,
    }
);

my %character = (
    1 => {
        name => "男主角",
        max_hp => 100,
        max_mp => 100,
        skill => [SKILL_1,SKILL_2,SKILL_3],
    },

    2 => {
        name => "女主角"
        max_hp => 100,
        max_mp => 100,
        skill => [SKILL_1,SKILL_2,SKILL_3],
    },

    3 => {
        name => "男配角"
        max_hp => 100,
        max_mp => 100,
        skill => [SKILL_1,SKILL_2,SKILL_3],
    },

    4 => {
        name => "女配角"
        max_hp => 100,
        max_mp => 100,
        skill => [SKILL_1,SKILL_2,SKILL_3],
    },
);

sub getRole {
    my ($id) = @_;
    my $role = $character{$id};
    my %output = map { $_ => $role->{$_} } keys %$role;
    my @skills = map { { id => $_, name => $skill{$_}{name}, mp => $skill{$_}{mp} } } @{$output{skill}};
    $output{skill} = \@skills;
    return \%output;
}

