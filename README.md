# При необходимости дополнительно установить Ingress-Nginx Controller
helm upgrade --install ingress-nginx ingress-nginx --repo https://kubernetes.github.io/ingress-nginx --namespace ingress-nginx --create-namespace

или

kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.10.1/deploy/static/provider/cloud/deploy.yaml


# Команды и результат
helm install hw10 k8s</br>
дождаться полной инициализации контейнера RabbitMQ и MassTransit в сервисах (~2 мин)</br>
helm uninstall hw10
