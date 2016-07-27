---------------------
UGroup V 1.2
by Standardverlag
Readme.txt
---------------------

Thank you for purchasing UGroup, a simple Unity3D tool which will enhance your 
productivity by keeping the hierarchy orderly!

---------------------
HOW TO INSTALL:
- Download from the asset store
- Click "Import"
- When opening a new project: just select "UGroup" from your package list to import

---------------------
HOW TO USE:
- Select one or more objects in your scene
- Select from the menu: Edit/Group to group the selected objects
- To group objects you can use the keyboard shortcut: Control+Shift+G (on a PC) 
  or Command+Shift+G (on a Mac) 

- Select one or more objects that are in a group
- Select from the menu: Edit/Ungroup to ungroup the selected objects
- To ungroup objects you can use the keyboard shortcut: Control+Shift+H (on a PC) 
  or Command+Shift+H (on a Mac)
 
ADVANCED USE:
- Select one or more objects in the scene
- Select from the menu: Edit/Group... to start the group wizard
- With the group wizard you can specify:
  1) The name of the group (default name: "Group")
  2) The position of the "group object" (default position: 0,0,0)
  Note: Selecting the group object allows you to easily transform the whole group
  You can optionally set the position to the center of all selected objects.
  3) The group tag 
  Note: You might want to add a custom tag. To do this, select "Add Tag..." 
  from the tag drop down menu. On top of the appearing list (in your inspector
  window) you will find your custom tags. Just type the group name you want next 
  to the last Element. Be careful not to confuse the tag list with the list of 
  layers. The tag list is the first entry in the tag manager (which also contains
  the layers).
- Hint: You can keep the UGroup window open by selecting "Group" instead of 
  "Group and Close" or "Close". You can also dock the window by dragging it.

TECHNICAL INFORMATION:
UGroup adds the selected items to a new parent. Therefore it does create an empty
game object for you. The position of this game object is the origin by default. 
Therefore the local and word position of the grouped objects will stay the same.
When ungrouping, UGroup removes the selected objects from all parents and places
them back on the top (root) level of the hierarchy. If the remaining game object 
(group) is empty, it will automatically delete it. With nested groups a group 
object might remain, which you can delete manually if desired.
  
---------------------
Version History:
 
 Version 1.2: - Changed the group wizzard into an editor window
              - Replaced the tag option with a tag drop down menu
              - Added option to center the group relative to the selected objects
              - After grouping automatically select the group object

 Version 1.1: - Added group wizard for some customization 

 Version 1.0: - Initial  

---------------------
Support:

http://unity.standardverlag.de
Mail: unity@standardverlag.de

Happy grouping! 
