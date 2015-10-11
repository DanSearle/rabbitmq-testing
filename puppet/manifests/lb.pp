class { 'haproxy': }
haproxy::listen { 'rabbit00':
    collect_exported => false,
    ipaddress        => '*',
    mode             => 'tcp',
    ports            => '5672',
    require          => Package['rabbitmq-server'],
}
haproxy::balancermember { 'rabbit1':
    listening_service => 'rabbit00',
    server_names      => 'rabbit1.local',
    ipaddresses       => '10.0.0.10',
    ports             => '5672',
    options           => 'check inter 5000 rise 2 fall 3',
}
haproxy::balancermember { 'rabbit2':
    listening_service => 'rabbit00',
    server_names      => 'rabbit2.local',
    ipaddresses       => '10.0.0.11',
    ports             => '5672',
    options           => 'check inter 5000 rise 2 fall 3',
}

package { 'rabbitmq-server': ensure => 'purged'; }
