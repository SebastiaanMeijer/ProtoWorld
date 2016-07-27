
            -- Things to run before way_ext 
            -- TODO: fix viewconnection_node to include nodes from node_stockholm table that are relative to traffic

            -- Connection node
            delete from connection_node;
            insert into connection_node
            select * from viewconnection_node;
            -- Network ways
            delete from networkways;
            insert into networkways
            select * from viewnetworkways;
            -- Way direction
            delete from way_direction;
            insert into way_direction
            select * from viewway_direction;