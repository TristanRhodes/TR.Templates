FROM ghcr.io/mpostument/grafana-sync:1.6.0

COPY launch.sh scripts/launch.sh
RUN chmod +x scripts/launch.sh
RUN dos2unix scripts/launch.sh

ENTRYPOINT ["sh", "scripts/launch.sh"]