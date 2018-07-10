#Store Transform PlayMode Changes Tool#

This is a simple tool that allows you to store any changes made to the Position, Rotation and Scale of all Gameobjects in the scene
during PlayMode, and apply those changes to all the Gameobjects after exiting PlayMode. Supports Undo and Redo operations as well.
Instructions:
* Place this script in the Editor folder in your project. Then the 'MyTools' menu should appear in your MenuBar with the 
option to open the TransformChangesTool window. The window has two buttons - Store Transform Changes and Apply Transform Changes.
The window is dockable so it can be placed anywhere in the editor.
In Play Mode the Store Transform Changes button is enabled. In Edit Mode the Apply Transform Changes button is enabled.
* Enter PlayMode. Make changes to the Postion, Rotation and Scale of gameobjects in the scene. Click 'Store Transform Changes'.
* Exit PlayMode. Click 'Apply Transform Changes'. All the changes you made to the Position, Rotation and Scale of any 
gameobjects in the scene, will be applied.
