# ��� ������������� ������������� ���������� Ingress-Nginx Controller
helm upgrade --install ingress-nginx ingress-nginx --repo https://kubernetes.github.io/ingress-nginx --namespace ingress-nginx --create-namespace

���

kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.10.1/deploy/static/provider/cloud/deploy.yaml


# ������� � ���������
helm install hw10 k8s
��������� ������ ������������� ���������� RabbitMQ � MassTransit � �������� (~2 ���)
helm uninstall hw10
