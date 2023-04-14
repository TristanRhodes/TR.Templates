#!/bin/bash

echo "Writing Dashboards"
grafana-sync pull-dashboards --directory="/etc/grafana/dashboards" --url http://admin:admin@Template.DbApi.Grafana:3000

echo "Grafana Export Complete"