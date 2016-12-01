:loop

REM sumo --remote-port 3456 -n map.net.xml -r map.rou.xml -a bus_flows.add.xml --fcd-output 127.0.0.1:3654 --fcd-output.geo --time-to-teleport 60

sumo -c map.local.sumocfg

@goto loop

