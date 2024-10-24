#!/bin/bash

# Verificar se o número correto de argumentos foi fornecido
if [ "$#" -ne 3 ]; then
    echo "Uso: $0 <porta> <alias> <proxy_pass>"
    exit 1
fi

PORTA=$1
ALIAS=$2
PROXY_PASS=$3
NAME_HOST=$4

# Definir o caminho para o arquivo de configuração do Nginx
CONFIG_FILE="/home/fouradmin/webserverteste/resultado/virtual.host.conf"

# Criar um texto de configuração padrão com base na entrada do usuário
if [ "$PORTA" -eq 443 ]; then
    CONFIG_TEXT=$(cat <<EOF
server $NAME_HOST {
        listen $PORTA ssl;
	ssl_trusted_certificate /etc/ssl/pmc/CA_BUNDLE.crt;
	ssl_certificate /etc/ssl/pmc/CA_BUNDLE.crt;
	ssl_certificate_key /root/contagem.mg.gov.br.key;
        server_name $ALIAS;
        location / {
            proxy_pass http://$PROXY_PASS;
            proxy_set_header Host \$host;
            proxy_set_header X-Real-IP \$remote_addr;
            proxy_set_header X-Fowarded-For \$proxy_add_x_forwarded_for;
            proxy_set_header X-Fowarded-Proto \$scheme;
        }
}


EOF
)
else
    CONFIG_TEXT=$(cat <<EOF
server $NAME_HOST {
        listen $PORTA;
        server_name $ALIAS;
        location / {
            proxy_pass http://$PROXY_PASS;
            proxy_set_header Host \$host;
            proxy_set_header X-Real-IP \$remote_addr;
            proxy_set_header X-Fowarded-For \$proxy_add_x_forwarded_for;
            proxy_set_header X-Fowarded-Proto \$scheme;
        }
}
EOF
)
fi

# Adicionar o texto de configuração ao final do arquivo
echo "$CONFIG_TEXT" >> "$CONFIG_FILE"

# Reiniciar o Nginx para aplicar as mudanças
# sudo systemctl reload nginx

echo "Configuração do Nginx atualizada com sucesso."
