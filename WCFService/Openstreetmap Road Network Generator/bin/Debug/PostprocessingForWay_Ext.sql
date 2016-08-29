			-- Things to run after populating way_ext table
			-- way ext node info aram
			delete from wayextnodeinfoaram_cachedfromview;
			insert into wayextnodeinfoaram_cachedfromview
			select * from viewwayextnodeinfoaram;