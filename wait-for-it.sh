#!/usr/bin/env bash
#   Use this script to test if a given TCP host/port are available

TIMEOUT=15
QUIET=0
PROGNAME=$(basename "$0")

echoerr() {
    if [[ "$QUIET" -ne 1 ]]; then echo "$@" 1>&2; fi
}

usage() {
    cat << USAGE >&2
Usage:
    $PROGNAME host:port [-t timeout] [-- command args]
    -q | --quiet                        Do not output any status messages
    -t TIMEOUT | --timeout=timeout      Timeout in seconds, zero for no timeout
    -- COMMAND ARGS                     Execute command with args after the test finishes
USAGE
    exit 1
}

wait_for() {
    if [[ "$TIMEOUT" -gt 0 ]]; then
        echoerr "$PROGNAME: waiting $TIMEOUT seconds for $HOST:$PORT"
    else
        echoerr "$PROGNAME: waiting for $HOST:$PORT without a timeout"
    fi
    start_ts=$(date +%s)
    while :
    do
        if nc -z "$HOST" "$PORT"; then
            end_ts=$(date +%s)
            echoerr "$PROGNAME: $HOST:$PORT is available after $((end_ts - start_ts)) seconds"
            break
        fi
        sleep 1
    done
    return 0
}

while [[ $# -gt 0 ]]
do
    case "$1" in
        *:* )
        HOST=$(printf "%s\n" "$1"| cut -d : -f 1)
        PORT=$(printf "%s\n" "$1"| cut -d : -f 2)
        shift 1
        ;;
        -q | --quiet)
        QUIET=1
        shift 1
        ;;
        -t)
        TIMEOUT="$2"
        if [[ "$TIMEOUT" = "" ]]; then break; fi
        shift 2
        ;;
        --timeout=*)
        TIMEOUT="${1#*=}"
        shift 1
        ;;
        --)
        shift
        break
        ;;
        --help)
        usage
        ;;
        *)
        echoerr "Unknown argument: $1"
        usage
        ;;
    esac
done

if [[ "$HOST" = "" || "$PORT" = "" ]]; then
    echoerr "Error: you need to provide a host and port to test."
    usage
fi

shift $((OPTIND-1))

wait_for

if [[ $? -eq 0 && $# -gt 0 ]]; then
    exec "$@"
else
    exit $?
fi