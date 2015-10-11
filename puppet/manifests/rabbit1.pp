class { 'rabbitmq':
  config_cluster           => true,
  cluster_nodes            => ['rabbit1', 'rabbit2'],
  cluster_node_type        => 'disc',
  erlang_cookie            => '2ac2f4145ee3803dd2209b89c2ffdb971d5c43b18ad9e9a63e6910ec4e2f932c',
  wipe_db_on_cookie_change => true,
}
host {
    'rabbit1':
        ip => '10.0.0.10';
    'rabbit2':
        ip => '10.0.0.11';
}
rabbitmq_user { 'admin':
    admin      => true,
    password => 'toor',
}
rabbitmq_user { 'dotnet':
    password => 'dotnet',
}
