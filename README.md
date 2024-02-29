# ePaper meetingroom display server

This repository contains the source code of the server that connects to the NOI meeting rooms calendar, takes the meeting information (e.g. meeting name, organizer name, start time, end time, etc.) and send it to the ePaper display installed in the meetingrooms. The server manages also the booking funtionality that allows the user to book the room, if available, by using the ePaper touch display installed in the meeting room.


# Build

`docker build . -t noi-door-signage`

# Startup

First of all create a .env file somewhere wit all needed env variables, which are:

- ASPNETCORE_ENVIRONMENT=Development
- ASPNETCORE_URLS=http://+:5010
- LOG_PATH=xxx
- LOG_LEVEL=Debug
- AZURE_TENANT_ID=xxx
- AZURE_CLIENT_ID=xxx
- AZURE_CLIENT_SECRET=xxx

Then start your container:\
`docker run --env-file <path to the .env> -e TZ=Europe/Rome -p 5010:5010 -t noi-door-signage`

In my personal NGinx configuration, I use this:
`server {
server_name noi-door-signage.codethecat.dev;

         location / {
                        proxy_pass http://localhost:5010;
                        proxy_http_version 1.1;
                        proxy_set_header Upgrade $http_upgrade;
                        proxy_set_header Connection 'Upgrade';
                        proxy_set_header Host $host;
                        proxy_cache_bypass $http_upgrade;
        }


    listen 443 ssl; # managed by Certbot
    ssl_certificate /etc/letsencrypt/live/noi-door-signage.codethecat.dev/fullchain.pem; # managed by Certbot
    ssl_certificate_key /etc/letsencrypt/live/noi-door-signage.codethecat.dev/privkey.pem; # managed by Certbot
    include /etc/letsencrypt/options-ssl-nginx.conf; # managed by Certbot
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem; # managed by Certbot

}`

## Room configuration:

Please check the appsettings.json and add the missing informations.
