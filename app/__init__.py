import os

from flask import Flask
from flask_cors import CORS

from .extensions import db, migrate


def create_app():
    app = Flask(__name__)

    CORS(
        app,
        origins=["http://localhost:3000", "http://127.0.0.1:3000"],
        methods=["GET", "POST", "PATCH", "DELETE", "OPTIONS"],
        allow_headers=["Content-Type"],
    )

    app.config["SQLALCHEMY_DATABASE_URI"] = os.environ["DATABASE_URL"]

    db.init_app(app)
    migrate.init_app(app, db)

    from .routes import api

    app.register_blueprint(api)

    @app.cli.command("clear-environments")
    def clear_environments():
        from .models import Environment

        db.session.execute(db.delete(Environment))
        db.session.commit()
        print("All environment records deleted.")

    return app
