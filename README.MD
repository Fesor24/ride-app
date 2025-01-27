# Soloride deployment with docker

## 1. After accessing the linux server, install the dependencies by running the following commands:

```bash
sudo apt update
sudo apt install docker.io nginx postgresql -y
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
sudo chmod 666 /var/run/docker.sock
sudo systemctl start docker
sudo systemctl enable docker
```

## 2. Authenticate the github account with the server either with ssh or by generating a personal access token:

    - Account Settings > Developer Settings > Personal Access Tokens > Tokens Classic > And generate a personal access token.

## 3. Clone the repo on the server:

```bash
git clone https://x-access-token:<access-token>@github.com/<account>/<repo>
```

replace: <access-token> with the github access token generated in step 2, <account>: with the github account username, <repo> with the name of the repo. E.g git clone https://x-access-token:ghu_dnkjbsdkjbbsdkjbdjbf@github.com/aitech-ng/solorideapp


## 4. cd into the cloned repo on the server and edit the appsettings.json:

```bash
cd Soloride-Backend-New
nano appsettings.json
```

paste what is in appsettings-template.json and delete the appsettings-template.json file

```bash
rm appsettings-template.json
```

## 5. Before deploying the docker container with docker-compose, configure the postgres database either on the existing server or a new server:

confirm if postgres is running:

```bash
sudo systemctl status postgresql
```

access postgres shell and create the database information:

```bash
sudo -u postgres psql
```

Run the following SQL commands:

```sql
CREATE DATABASE soloride;

CREATE USER databaseuser WITH ENCRYPTED PASSWORD 'strongPassword#001';

ALTER DATABASE soloride OWNER TO databaseuser;
```

Use more secure database credentials

## 6. Be sure to update the information created in the appsettings.json under `Database ConnectionString` and let the server ip address be in the `Server` section

## 7. Start up the docker containers and deploy the application in the main project directory:

```bash
docker-compose up -d
```