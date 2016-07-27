@echo off

REM OSM TO DATABASE POPULATOR
REM ---------------------------------------------------------------------------------
REM
REM This batch creates a PostgreSQL database and populates it with OSM data, making 
REM all the pre and post-processing needed to get it ready for ProtoWorld. 
REM
REM To invoke it, run in a console line with the following arguments 
REM (all of them mandatory):
REM
REM Argument 1: Database name (no spaces)
REM Argument 2: Path of the OSM file to load

REM - IMPORTANT!! 
REM - Configure the following parameters before running the script:
REM ---------------------------------------------------------------------------------
@set PATH=C:\Program Files\PostgreSQL\9.3\bin;C:\Program Files\PostgreSQL\9.3\extra_dep
@set URL=127.0.0.1
@set PGPORT=5432
@set PGHOST=localhost
@set PGUSER=postgres
@set PGPASSWORD=test
REM ---------------------------------------------------------------------------------

REM -- Fixed parameters --
@SET PGDATABASE="%~1"
@SET LOAD_FILE="%~2"
@SET SETUP_FILE=%~dp0\Query1.sql
@SET POP_FILE=%~dp0\Query2.sql

REM -- Preparing the database --
echo Preparing the database...
dropdb -U %PGUSER% %PGDATABASE%
createdb -U %PGUSER% %PGDATABASE%
psql -U %PGUSER% -d %PGDATABASE% -c "create extension postgis;"
psql -U %PGUSER% -d %PGDATABASE% -c "create extension pgrouting;"

REM -- Set schema --
echo Setting schema...
psql -U %PGUSER% -d %PGDATABASE% -f "%SETUP_FILE%"

REM -- Populate osm database --
echo Populating OSM database...
"..\..\..\WCFService\OSM Populate database\bin\x86\Debug\OSM Populate database.exe" %LOAD_FILE% "Server=%URL%;Port=%PGPORT%;Database=%PGDATABASE%;User Id=%PGUSER%;Password=%PGPASSWORD%;"

REM -- Populate full extent -- 
echo Post-processing...
psql -U %PGUSER% -d %PGDATABASE% -f "%POP_FILE%"

echo Process completed!

REM pause