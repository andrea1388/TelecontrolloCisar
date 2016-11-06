# TelecontrolloCisar
Software per raspberry per controllare da remoto dei relays 


Configurazione apache (2.4)

Abilitare i moduli ssl e rewrite (il primo per abilitare https il secondo per fare il redirect delle richieste http su https)

Per generare il certificato (da usare con il protocollo https)
cd /root
mkdir certssl
cd certssl
openssl req -newkey rsa:1024 -x509 -days 9650 -nodes -out cert.pem -keyout key.pem

Modifiche al server virtuale *.80
aggiungere le direttive:
RewriteEngine On
RewriteCond %{HTTPS} off
RewriteRule ^ https://%{HTTP_HOST}%{REQUEST_URI}
Modifiche al server virtuale *.443
aggiungere le direttive:
SSLEngine on
SSLCertificateFile /root/certssl/cert.pem
SSLCertificateKeyFile /root/certssl/key.pem

ln -s cl /bin/cl
sudo apt-get install apache2 -y
sudo apt-get install php5 libapache2-mod-php5 -y

Aggiungere a /etc/dhcpcd.conf
interface eth0
static ip_address=10.26.1.14/24
static routers=10.26.1.1
static domain_name_servers=10.26.1.1

/etc/network/interfaces deve contenere
auto eth0
  iface eth0 inet manual

per mettere i log su disco ram, in /etc/fstab, aggiungere
  tmpfs	/var/log	tmpfs	size=3000000	0	0


Creare un servizio, attivo al boot:

[Unit]
Description=mklogdir4apache2
After=local-fs.target

[Service]
ExecStart=/bin/mkdir /var/log/apache2

[Install]
WantedBy=multi-user.target