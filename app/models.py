from sqlalchemy import inspect

from .extensions import db

import sqlalchemy as sq
from sqlalchemy.orm import Mapped, mapped_column

class Environment(db.Model):
    __tablename__ = "environments"

    id = mapped_column(sq.Integer, primary_key=True)

    site_code = mapped_column(sq.String(3))
    classification_code = mapped_column(sq.String(3))

    # Global values
    ca_crt = mapped_column(sq.Text())
    ntp_server = mapped_column(sq.String(255))


    # K8s values
    
    k8s_namespace = mapped_column(sq.String(100), default="ges-namespace")
    k8s_cluster = mapped_column(sq.String(100), default="ges-cluster" )
    k8s_pod_cidr = mapped_column(sq.String(255), default="172.69.0.0/16")
    k8s_service_cidr = mapped_column(sq.String(255), default="172.169.0.0/16")
    k8s_vkr_version = mapped_column(sq.String(255), default="1.32.0")
    k8s_storage_class = mapped_column(sq.String(255), default="vsan_default_storage_class")

    # Control Plane
    k8s_cp_nodes = mapped_column(sq.Integer, default=3)
    k8s_cp_vm_class = mapped_column(sq.String(255), default="guaranteed-large")

    # Workers
    k8s_worker_nodes = mapped_column(sq.Integer, default=3)
    k8s_worker_vm_class = mapped_column(sq.String(255), default="guaranteed-large")

    @property 
    def name(self):
        return f"{self.site_code}-{self.classification_code}"

    @property
    def harbor_url(self):
        return f"harbor.bigsafari.{self.site_code}.usaf"

    def to_dict(self):
        data = {
            attr.key: getattr(self, attr.key)
            for attr in inspect(type(self)).column_attrs
        }

        data["name"] = self.name
        data["harbor_url"] = self.harbor_url

        return data
