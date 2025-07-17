# View Data in Kibana

## 1. Create Index Pattern in Kibana
- Open Kibana in browser `http://localhost:5601/`
- Click menu `Stack Management` → `Data Views` (or `Index Patterns`).
- Click `Create data view`.
- Fill:
    Name: `otel-logs-*`
    Index pattern: `otel-logs-*`
    Timestamp field: Select @timestamp (or empty if not any).

- Click `Create data view`.

## 2. Show Log di Kibana
- Open menu `Discover`.
- Select data view otel-logs-*.
- Then you can:
    - Filter based on index like `otel-logs-order-service`.
    - Or using query body: `something`.

# Troubleshoot
```bash
# Check elasticsearch up and running
curl http://localhost:9200/

# check indices in elasticsearch
curl -s 'http://localhost:9200/_cat/indices?v'

# check log opentelemetry collector
docker logs src-otel-collector-1
```

# Create Retention in Elasticsearch
The steps are:
- Create  ILM (Index Lifecycle Management) policy

    ```bash
    # Example: retention 7 days and rollover every 1 day / 50GB: 
    curl -X PUT http://localhost:9200/_ilm/policy/logs-retention-policy -H 'Content-Type: application/json' -d '
    {
      "policy": {
        "phases": {
          "hot": {
            "actions": {
              "rollover": {
                "max_age": "1d",
                "max_size": "50gb"
              }
            }
          },
          "delete": {
            "min_age": "7d",
            "actions": {
              "delete": {}
            }
          }
        }
      }
    }'

    ```

- Applying existing index `otel-logs-customer-service` to the `logs-retention-policy`
    
    ```bash
    curl -X PUT "http://localhost:9200/otel-logs-customer-service/_settings" -H 'Content-Type: application/json' -d '
    {
      "index": {
        "lifecycle": {
          "name": "logs-retention-policy"
        }
      }
    }'
    ```

    Then check the index information
    ```bash
    curl -X GET 'http://localhost:9200/otel-logs-customer-service/_ilm/explain?pretty'

    # output
    {
      "indices" : {
        "otel-logs-customer-service" : {
          "index" : "otel-logs-customer-service",
          "managed" : true,
          "policy" : "logs-retention-policy",
          "index_creation_date_millis" : 1752349344101,
          "time_since_index_creation" : "15.59m",
          "lifecycle_date_millis" : 1752349344101,
          "age" : "15.59m",
          "phase" : "hot",
          "phase_time_millis" : 1752350268549,
          "action" : "rollover",
          "action_time_millis" : 1752350268949,
          "step" : "check-rollover-ready",
          "step_time_millis" : 1752350268949,
          "phase_execution" : {
            "policy" : "logs-retention-policy",
            "phase_definition" : {
              "min_age" : "0ms",
              "actions" : {
                "rollover" : {
                  "max_age" : "1d",
                  "max_primary_shard_docs" : 200000000,
                  "min_docs" : 1,
                  "max_size" : "50gb"
                }
              }
            },
            "version" : 1,
            "modified_date_in_millis" : 1752349590171
          }
        }
      }
    }

    ```

    You can see
    ```json
    {
      "indices" : {
        "otel-logs-customer-service" : {
          "index" : "otel-logs-customer-service",
          "managed" : true,
          "policy" : "logs-retention-policy",
           ....
           ....
        }
      }
    }
    ```

- Do the same to other microservice log index `otel-logs-order-service`

# Troubleshooting

```bash
docker exec -it alpine-debug sh

/$ apk update && apk add curl
```