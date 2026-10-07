import json
from flask import Blueprint, abort, request
from sqlalchemy.exc import IntegrityError

from .cluster_access import NoContextException, get_client, cluster_health
from .extensions import db
from .models import Environment
import jinja2

api = Blueprint("api", __name__)


def read_data():
    data = request.get_json()

    if not isinstance(data, dict):
        abort(400, description="Expected a JSON object")

    return data


def save_changes():
    try:
        db.session.commit()
    except IntegrityError:
        db.session.rollback()
        abort(409, description="An environment with that name already exists")


############
# Clusters #
############
@api.get("/clusters/<string:cluster_ctx>/health")
def get_cluster_health(cluster_ctx: str):
    try: 
        client = get_client(cluster_ctx)
        health = cluster_health(client)
        return health
    except NoContextException as err:
        return { "healthy": False, "details": str(err)}

############
# Database #
############


@api.post("/environments")
def create_environment():
    data = read_data()

    environment = Environment(**data)
    db.session.add(environment)

    save_changes()

    return environment.to_dict(), 201


@api.get("/environments")
def list_environments():
    environments = db.session.scalars(
        db.select(Environment).order_by(Environment.id)
    ).all()

    return [environment.to_dict() for environment in environments]


@api.get("/environments/<int:environment_id>")
def get_environment(environment_id: int):
    return db.get_or_404(Environment, environment_id).to_dict()


@api.get("/environments/cluster/<int:environment_id>")
def generate_cluster_yml(environment_id: int):
    env = db.get_or_404(Environment, environment_id).to_dict()

    jinja = jinja2.Environment(loader=jinja2.FileSystemLoader("templates/"))
    template = jinja.get_template("cluster.yml")

    rendered = template.render(model=env)
    return rendered


@api.patch("/environments/<int:environment_id>")
def update_environment(environment_id: int):
    environment = db.get_or_404(Environment, environment_id)

    for field, value in read_data().items():
        setattr(environment, field, value)

    save_changes()
    return environment.to_dict()


@api.delete("/environments/<int:environment_id>")
def delete_environment(environment_id: int):
    environment = db.get_or_404(Environment, environment_id)
    db.session.delete(environment)
    db.session.commit()

    return "", 204
