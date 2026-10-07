import ast
import subprocess
import kubernetes as k
from kubernetes.client import ApiException, CoreV1Api
from requests import HTTPError

def main():
    try:
        super = get_client("k8s-ns")
        nodes = super.list_node()
        print(nodes)
    except Exception as err:
        print(err)

def get_client(context: str) -> CoreV1Api: 
    k.config.load_kube_config()
    if _context_exists(context):
        return CoreV1Api(api_client = k.config.new_client_from_config(context=context))
    else:
        raise NoContextException(f"The cluster context {context} does not exist")

def _context_exists(context: str) -> bool:
    contexts, _ = k.config.list_kube_config_contexts()

    for ctx in contexts:
        if ctx['name'] == context:
            return True
    return False

def cluster_health(client: CoreV1Api) -> dict:
    try:
        result = client.api_client.call_api(
            resource_path="/healthz",
            method="GET",
            header_params={"Accept": "text/plain"},
            auth_settings=["BearerToken"],
            response_types_map={200: "str"},
            query_params=[("verbose", "")],
            _return_http_data_only=True,
            _request_timeout=10,
        )

        details = (
            result.decode("utf-8")
            if isinstance(result, bytes)
            else result
        )

        return {"healthy": True, "details": details}

    except ApiException as exc:
        details = exc.body or str(exc)
        if isinstance(details, bytes):
            details = details.decode("utf-8", errors="replace")

        return {"healthy": False, "details": details}

    except HTTPError as exc:
        return {"healthy": False, "details": str(exc)}

# config.load_kube_config()
# v1 = client.CoreV1Api()
# print("Listing pods with their IPs:")
# ret = v1.list_pod_for_all_namespaces(watch=False)
# for i in ret.items:
#     print("%s\t%s\t%s" % (i.status.pod_ip, i.metadata.namespace, i.metadata.name))

class NoContextException(Exception):
    pass

if __name__ == '__main__':
    main()
