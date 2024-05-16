> [!WARNING]
> This plugin is in beta.

# SignInSign
- Author：Soofa、羽学、少司命
- Source: [github](https://github.com/Soof4/PvPer/)  
- SignInSign turns logging in and registering command process into UI process with sign interface.
- Players can pop up a notice board when entering the server, which can be used to register, log in, obtain items, BUFF, teleport, record player passwords, and other functions
- Attention:
- Unable to communicate with<Chireiden.TShock.Omni>[plugin](https://github.com/sgkoishi/yaaiomni/) together, otherwise the tourist registration and login function will be invalidated.
- Installing this plugin will prevent players from destroying all billboards on the server unless they have sign. edit permission.
- The plugin will create a hidden billboard at the birth point based on the content of the configuration file
- To change the content of the billboard:
- 1. Use logged in roles in the server to dig up billboards
- 2. Modify the content of creating a billboard in the [SignInSign.json] configuration file
- 3. Input command:/gs r

## Permissions
| Permissions      | Commands            |illustrate            |
|------------------|---------------------|---------------------|
| signinsign.setup | /gs r         | Reset Sign |
| signinsign.setup | /gs s           |Set up Teleport points on billboards|
| sign.edit | None           | Allow violation of Sign permission        |

## Update Log
```
1.0.3
少司命 has fixed the issue of multiple people entering the server without a pop-up window on the billboard, and added a feature to prevent damage and modification of the billboard grid

羽学 optimized the instructions, using/gs s to quickly set the current position as the transfer point, and using/gs r to reset the billboard (automatic execution/reload)

Added detection of player login, turn on teleportation switch, and non zero teleportation point coordinates for billboard teleportation

1.0.2
Fixed some bugs with empty references
Added Reload method and a large number of configuration items
```

## Config
```json
{
  "Is the registration and login function enabled": true,
  "Record role passwords": false,
  "Display billboards for logged in players": true,
  "Is it allowed to click Sign": true,
  "Click Sign to confirm whether to send a broadcast": false,
  "Create content for Sign and reset instructions:/gs r": "Welcome to my server. \nThis server supports chain mining, more crystal towers can be placed and used. NPCs sell more items, and more materials can be converted into low light. There are also RPG professional stores and magic modified bosses waiting for you to experience! \nPlease type in your password below the line：\n",
  "Click on Sign's broadcast": "Enter 2 times in sequence in this Sign：\n[c/F7CCF0:123456]  Register and log in",
  "Attempting to disrupt Sign's broadcast": "This Sign cannot be modified!",
  "What command to execute by clicking Sign": [],
  "What BUFF is given by clicking Sign": [],
  "Click on SignBUFF duration/minute": 10,
  "What item to give by clicking Sign": [],
  "Click Sign to give the quantity of items": 1,
  "Click on Sign to confirm transmission and set instructions:/gs s": false,
  "Click Sign to Teleport_X": 0.0,
  "Click Sign to Teleport_Y": 0.0,
  "Click on Sign to style": 10
}
```