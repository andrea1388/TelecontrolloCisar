/*
Simple udp client
Silver Moon (m00n.silv3r@gmail.com)
*/
#include<stdio.h> //printf
#include<string.h> //memset
#include<stdlib.h> //exit(0);
#include<arpa/inet.h>
#include<sys/socket.h>

#define SERVER "127.0.0.1"
#define BUFLEN 512  //Max length of buffer
#define PORT 11000   //The port on which to send data

void die(char *s)
{
	perror(s);
	exit(1);
}

int main(int argc, char *argv[])
{
	struct sockaddr_in si_other;
	int k, s, i, slen = sizeof(si_other);
	char buf[BUFLEN];
	char message[BUFLEN];

	if (argc<2)
	{
		printf("cl <on|off> <linea>\n");
		return 1;
	}
	memset(buf, '\0', BUFLEN);

	for (k = 1; k<argc; k++)
	{
		strcat(buf, argv[k]);
		if (k<argc - 1) strcat(buf, " ");
	}
	//printf("cmd: (%s)%s",buf, "\n");
	if ((s = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) == -1)
	{
		die("socket");
	}

	memset((char *)&si_other, 0, sizeof(si_other));
	si_other.sin_family = AF_INET;
	si_other.sin_port = htons(PORT);

	if (inet_aton(SERVER, &si_other.sin_addr) == 0)
	{
		fprintf(stderr, "inet_aton() failed\n");
		exit(1);
	}


	//send the message
	if (sendto(s, buf, strlen(buf), 0, (struct sockaddr *) &si_other, slen) == -1)
	{
		die("sendto()");
	}

	//receive a reply and print it
	//clear the buffer by filling null, it might have previously received data
	memset(buf, '\0', BUFLEN);
	//try to receive some data, this is a blocking call
	if (recvfrom(s, buf, BUFLEN, 0, (struct sockaddr *) &si_other, &slen) == -1)
	{
		die("recvfrom()");
	}

	puts(buf);

	close(s);
	return 0;
}