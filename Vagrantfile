# -*- mode: ruby -*-
# vi: set ft=ruby :

# Vagrantfile API/syntax version. Don't touch unless you know what you're doing!
VAGRANTFILE_API_VERSION = "2"

Vagrant.configure(VAGRANTFILE_API_VERSION) do |config|
  config.vm.box = "debian/jessie64"
  config.vm.provision "shell", inline: "apt-get update && apt-get install -y puppet"


  config.vm.define "rabbit1" do |rabbit1|
   rabbit1.vm.network :private_network, ip: "10.0.0.10"
   rabbit1.vm.hostname = "rabbit1"
    rabbit1.vm.provision :puppet do |puppet|
        puppet.manifests_path = "puppet/manifests"
        puppet.module_path    = "puppet/modules"
        puppet.manifest_file  = "rabbit1.pp"
    end
  end
  config.vm.define "rabbit2" do |rabbit2|
   rabbit2.vm.network :private_network, ip: "10.0.0.11"
   rabbit2.vm.hostname = "rabbit2"
    rabbit2.vm.provision :puppet do |puppet|
        puppet.manifests_path = "puppet/manifests"
        puppet.module_path    = "puppet/modules"
        puppet.manifest_file  = "rabbit2.pp"
    end
  end
  config.vm.define "loadbalancer" do |loadbalancer|
    loadbalancer.vm.network :private_network, ip: "10.0.0.12"
    loadbalancer.vm.provision :puppet do |puppet|
        puppet.manifests_path = "puppet/manifests"
        puppet.module_path    = "puppet/modules"
        puppet.manifest_file  = "lb.pp"
    end
    loadbalancer.vm.hostname = "loadbalancer"
  end
  config.vm.provider "virtualbox" do |v|
    v.customize ["modifyvm", :id, "--cpuexecutioncap", "50"]
    v.memory = 256
    v.cpus = 1
  end

end
