from sqlalchemy import inspect

from .extensions import db


class Environment(db.Model):
    __tablename__ = "environments"

    id = db.Column(db.Integer, primary_key=True)

    site_code = db.Column(db.String(3))
    classification_code = db.Column(db.String(3))

    # private
    custom_name = db.Column(db.String(255))
    custom_harbor_url = db.Column(db.String(255))

    ca_crt = db.Column(db.Text())
    
    k8s_namespace = db.Column(
        db.String(100),
        default="ges-namespace",
        server_default="ges-namespace",
    )
    
    k8s_cluster = db.Column(
        db.String(100),
        default="ges-cluster",
        server_default="ges-cluster",
    )

    @property 
    def name(self):
        if self.custom_name:
            return self.custom_name
        elif self.site_code and self.classification_code:
            return f"{self.site_code}-{self.classification_code}"
        else:
            return None

    @property
    def harbor_url(self):
        if self.custom_harbor_url:
            return self.custom_harbor_url
        elif self.site_code:
            return f"harbor.bigsafari.{self.site_code}.usaf"
        else:
            return None

    def to_dict(self):
        data = {
            attr.key: getattr(self, attr.key)
            for attr in inspect(type(self)).column_attrs
        }

        data["name"] = self.name

        return data

