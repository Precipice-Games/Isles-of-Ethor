# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
* Put any unreleased features here.

## [v2.2.4] - 2026-05-07

### Added
* Implemented Flower Island NPC dialogue.
* Added logic for flower pickups on Flower Island. 

### Fixed
* Fixed a bug where the Player was not moving with the moving parkour platforms on Oasis Island.

### Changed
* Updated the rotation of Oasis Island puzzle 3.

## [v2.2.3] - 2026-05-05

### Added
* Added baseline weather particle systems for Ice Island and Oasis Island.
* Added some scene barriers to Ice Island and Oasis Island to prevent the Player from walking directly past the puzzles.

### Fixed
* Fixed a bug where the feedback system was broken after 0c3f7f3.

### Changed
* Updated the rotation of a few Oasis Island puzzles.
* Restored feedback system for all islands' puzzle prefabs.
* Restored in-progress gust mechanic code.

## [v2.2.2] - 2026-05-04

### Added
* Added background music to Flower Island.
* Created new prefab for Flower Island named Created FlowerBasePuzzleNew.prefab.
* Added all Ice Island tree variations.

### Fixed
* Fixed a bug where many Players were unable to move on high-end devices.
     * Still a few bugs with this feature but significantly better than what was patched in 2f95294.
* Fixed a bug where the onscreen puzzle feedback wasn't appearing with finalized style from v2.1.4.
* Fixed a bug where the material for "normal" tiles on Ice Island wasn't appearing.

### Changed
* Restored some systems and scripts wiped after 0c3f7f3.
* PlayerMovement.cs logic has been further spliced up into more update-based methods.
* Updated Mother Island death boxes to handle dynamic respawning in relation to puzzle progression.

## [v2.2.1] - 2026-05-02

### Added
* Created first draft of RopeBridge.prefab (model made by @Samisushi).
* Added PostProcessing.prefab to Mother_Island.unity.

### Fixed
* Performed checks to ensure all in-game puzzles are correct with tile layouts, resource data, and functionality.
* Fixed a bug where the gameplay wasn't blurring when paused on Mother Island.

### Changed
* Rune circles are no longer a subscriber of the Player interaction event.

## [v2.2.0] - 2026-05-01

### Added
* Added the Flower Island to the game.
* Added the new airship model to the game (made by @fwooley).

### Fixed
* Fixed a massive memory leak on Oasis Island in relation to the PuzzleCheatSign.

### Changed
* Island traversal chain has now been updated to include Flower_Island.unity.
* Oasis_Island.unity now goes to Flower_Island.unity instead of Mother_Island.unity.

## [v2.1.4] - 2026-04-30

### Fixed
* Fixed a bug with a missing reference to the Player in ExitPuzzleButton.cs.
     * This bug soft locked the Player in a broken puzzle.

### Changed
* Updated puzzle feedback UI to utilize the same text asset (aesthetic) of the interaction prompt.
* Reworked event handling for puzzle feedback.

## [v2.1.3] - 2026-04-29

### Fixed
* Fixed a bug where Ice Island puzzle 4 was unsolvable.
* Fixed a bug where the 'move down' card values for Oasis Island puzzle 1 were incorrect.

### Changed
* Increased the camera FOV on Ice Island puzzle 4.

## [v2.1.3] - 2026-04-29

### Fixed
* Fixed a bug where Ice Island puzzle 4 was unsolvable.
* Fixed a bug where the 'move down' card values for Oasis Island puzzle 1 were incorrect.

### Changed
* Increased the camera FOV on Ice Island puzzle 4.

## [v2.1.2] - 2026-04-27

### Fixed
* Fixed a bug where the Player could not teleport to the Mother Island interior.

## [v2.1.1] - 2026-04-27

### Added
* Added black overlay for cutscene at the start of Mother Island.
* Added teleportation to the Oasis Island (after collecting crystal).
* Added walking and jumping animations to Skye.
* Added running trail at the end of Skye's walking animation.
* Added death box to Mother Island and Oasis Island.

### Fixed
* Fixed a bug where Skye was sometimes underneath the puzzle tiles.

### Changed
* Finalized Mother Island puzzles.
* Updated puzzle cheat sign to encompass crystal collection.
* Finalized UI SFX for alpha release.
* Improved "normal" tile texturing for greater visibility.
* Updated the positioning of the Ice Island collectable crystal.

## [v2.1.0] - 2026-04-26

### Added
* Implemented airship loading screen between all scene transitions (animation by @fwooley).
* Added all finalized puzzles to Oasis Island.
* Created dynamic respawning system for death boxes.
     * Implemented death boxes on Ice Island.
* Created FlowerBasePuzzle.prefab (not yet implemented).
* Added two public helper methods in Player.cs for toggling the CharacterController and teleporting the Player to a Vector3 location.
* Created GameManager.cs, which is currently in use for overarching scene changes.
* Imported enhanced skyboxes for Mother, Ice, and Oasis (made by @fwooley).
* Created "normal" tile type textures for Mother and Oasis.

### Fixed
* Restored jumping and ground checking functionality to the Player.
* Fixed a card usage bug where the backend logic was checking starting values rather than current values.
* Fixed a bug regarding event chaining for rune circle teleportation.
* Resolved a series of NullReferenceException errors with the audio sliders.
* Fixed button positioning in the UI.
* Fixed a bug where clicking the "Main Menu" button on the Mother Island would just restart the Mother Island scene.
* Fixed a bug where the "Resume" button on Oasis Island would transition you to the wrong state.

### Changed
* SceneChangerButton.cs is now SceneChanger.cs for greater availability to other classes.
     * Also, the class now contains the new queueLoadingScreen event, which is what drives the airship transition.
* Updated ViewManager.cs to notify GameManager.cs of when to switch scenes (animation completed).
* Player now teleports to start of Ice Island upon crystal collection (need this for other islands).
* Improved toggling of interaction prompt based on game states.
* Increased underlay of interaction prompt text for enhanced visual accessibility.
* Updated UI typography with new "Iosevka" font face.
* Dynamic tile texturing system has been expanded into sub-classes that suit each island's prefab.
* Updated puzzle UI with new assets.
* Moved puzzle UI to the righthand side of the screen instead of the bottom.
* Updated Mother Island model with finalized texture (made by @torialle).
* Updated rune circle appearance for greater contrast and visibility in the scene.
* Organized scene hierarchy of Ice, Oasis, and Mother islands.
* Updated Player.prefab groundcasting data.

## [v2.0.3] - 2026-04-26

### Added
* Implemented airship model on all islands.
     * Created an airship walkway in Unity to help the Player board the ship.
* Created three new unpacked prefabs out of Oasis Island .fbx models (made by @Samisushi).
* Added animated sprites to Oasis Island NPCs.

### Fixed
* Fixed a bug where Judith was floating in the air at the top of the mountain on Ice Island.
* Fixed a bug where the Player could not jump due to the changes introduced by the new CharacterController system.
* Fixed a bug where the Player could not "board" the airship due to ground checking bugs.

### Changed
* Completed airship connection between Mother, Ice, and Oasis Islands.
* Finalized Ice Island dialogue for Judith.
* Updated metadata of all build profiles to contain correct icon, title, and company.
* Updated some of Puzzle 2 on Oasis Island (WIP).
* Updated Oasis Island positioning with respect to the world origin.
* Adjusted dialogue box to be slightly transparent (by 10%).
* Corrected build order to load Mother Island as the first island.
* Refactored SceneChangerButton.cs with serialized Enum for clearer and safer manipulation in the Inspector.
* Disabled the BoxCollider component on the Player.prefab.

## [v2.0.2] - 2026-04-20

### Added
* Started adding interior build to Mother Island.
* Added localized game state handling with a non-static variable for small state-based tweaks inside PlayerMovement.cs.

### Fixed
* Fixed a bug where the card usages were not restoring to their initialized values when hitting Reset.
* Fixed a bug where Skye was not facing upwards (90° on x-axis) during puzzle mode, making her impossible to see.
* Fixed a bug where the PlayerInput component got disconnected from the Player.

### Changed
* Moved the toggling of the CharacterController from Player.cs into PlayerMovement.cs.

## [v2.0.1] - 2026-04-20

### Added
* Created development build profile for Windows.
* Added C# toggling CharacterController component when entering puzzle mode.
* Added cacti and other foliage to Oasis Island (made by @fwooley).
* Added title art and backdrop to menu (made by @fwooley).
* Added the rune circle asset to the puzzle prefabs (made by @Sparta437).
* Added art for dialogue box (made by @bobbysaville18-beep)

### Fixed
* Fixed the infinite mana resource bug.
* Fixed a bug where the Player couldn't select tiles in puzzle mode in some scenes.
* Fixed a bug where there was "ghost tiles" on the grid even after resetting a puzzle.
* Fixed a bug where the Player could not click puzzle tiles on Oasis Island.
* Fixed a bug to ensure proper exiting from all UI views.
* Resolved controls missing from the Settings UI.

### Changed
* Updated input handling configurations for all build profiles and general project settings.
* Finalized Player navigation and turning handling in Exploration mode.

## [v2.0.0] - 2026-04-15

### Added
* Created functional Airship that allows Players to travel between islands.
     * Added stand-in platform to represent airship (waiting on art).
* Developed an in-editor workaround to display puzzle layouts before runtime.
* Created the SFXManager to handle audio separate from the MusicManager.
* Added invalid move SFX if the Player tries to perform an illegal action during puzzle mode.
* Started setting up the physical layout of the Mother Island with the model made by @torialle.
* Added purple button graphics to UI.
* Created an occupancy map dictionary in GridManager.cs to restore initialization data upon resetting a puzzle.
* Added Julien and Mortaniel NPCs to Oasis Island.
* Created CompletionCrystal.prefab for improved scripting interaction.
* Added a fail-safe for [ExecuteAlways] property.
* Added the Ethor crystal icon to the Windows build profile (created by @fwooley).

### Fixed
* Fixed the infinite mana resource bug.
* Fixed a bug where the Player couldn't select tiles in puzzle mode in some scenes.
* Fixed a bug where there was "ghost tiles" on the grid even after resetting a puzzle.

### Changed
* Finalized Ice Island map and puzzles.
* Updated Windows and macOS build profiles with Mother_Island.unity and Oasis_Island.unity.
* Renamed BoxCastGrounded.cs script to be PlayerGroundcast.cs.
* Updated the grounded raycast to encompass boarding the airship.

### Removed
* Removed some unused audio assets.

## [v1.1.2] - 2026-04-09

### Added
* Started creating Mana Well tile (not yet available).
* Added Cinemachine 3.1.6 to the project.

### Fixed
* Created a solid workaround for cross-platform mouse polling inconsistencies.
* Fixed a bug where the different puzzle prefabs led to puzzle 2 on Ice Island being extremely behind puzzle 1's features.
* Fixed a bug where Skye was not visible from the overhead view of puzzle 2 on Ice Island.

### Changed
* Updated Penguin NPC to have white paper outline that matches other characters (made by @fwooley).
* Replaced all puzzle instantiations with new base puzzle prefabs (one prefab per island).
* Converted all normal Unity cameras into CinemachineCameras.
* Updated Game State Manager scripts to handle new CinemachineCameras.

### Removed
* Deleted camera rotation scripts.
* Deleted Puzzle2.prefab (originally intended for Ice Island).
* Removed puzzle 2 pillar from Ice_Island.unity.

## [v1.1.1] - 2026-04-07

### Added
* Implemented ice tree art by @fwooley.
* Added puzzle mode to Music Manager to play soundtracks separate from the standard scene music.
* Added some prototype feedback text as part of puzzle feedback system (WIP).

### Fixed
* Fixed an issue where the Ice Island was overexposed with too many light sources.
* Fixed an issue where the Player was not able to jump on regions of the ice island following puzzle 1.

### Changed
* Adjusted lighting and island materials in the Ice Island.
* Replaced the prototype trees on the Ice Island with the finalized ice trees.

## [v1.1.0] - 2026-04-05

### Added
* Implemented winter sky box on Ice Island.
* Added Judith artwork & sprite animation scripting.
* Created baseline puzzle prefab for quick future iteration.
* Started the construction of the Mother Island (not yet available).
* Added more prefabs and an EventSystem handler to Oasis Island.

### Fixed
* Fixed a bug where the Player kept rotating in Puzzle mode.
* Fixed a bug where the camera polling rate was causing significant lagging effects on different devices.
* Fixed an InputActions bug where the Player couldn't select tiles in Puzzle mode.
* Fixed a bug where Skye was bouncing and/or being slingshotted into when being teleported to the rune circle end tile after completing a puzzle.
* Fixed a bug where the Mana was being consumed for invalid moves.

### Changed
* Converted left click input in TileSelector.cs to InputActions.CallbackContext event stream.
* Updated InputManager.cs to listen for left click inputs.
* Updated YarnSpinner with new dialogue from the writers.
* Updated comments & dev serializations for ResourceManager.cs, TileSelector.cs, and GridManager.cs.
* Changed model of original ice island tiles to modern iceberg model (made by @Samisushi).
* Tweaked the size of rune circles' collision boxes.
* Updated the puzzle feedback system with new events (moveOutOfBounds).

### Removed
* Deleted TileMovement.cs.
* Deleted ClickRayTest.cs.
* Cleaned out some unused assets during sprint 8.
* Removed the old puzzle terminals from Ice Island hierarchy.
     * There might still be some left over from merge conflict #300.
* Removed duplicate PenguinBack sprite.

## [v1.0.3] - 2026-04-02

### Added
* Created URP post-processing system for real-time camera blur during Paused mode.
* Added the correct start and end tile systems to meet design goals.

### Fixed
* Fixed a bug where the Player kept rotating in Puzzle mode.
* Fixed a bug where the camera polling rate was causing significant lagging effects on different devices.

### Changed
* Changed puzzle terminals to be rune circles.
* Updated GridManager.cs to account for additional rows for new start and end tiles.
* Refined camera rotation system to be frame-independent and work with the input system callback context events.
* PlayerMovement.cs now handles Player rotation instead of CameraRotation.cs.

## [v1.0.2] - 2026-03-29

### Added
* Added a reset button to puzzles in puzzle mode.
* Added the ability to exit puzzles from puzzle mode back into exploration mode.
* Added Oasis Island scene (not yet in build settings, but present in the project folder).
* Created PuzzleFeedback.cs with a prototype "error flashing" system on tiles attempting to move into occupied cells (WIP).

### Fixed
* Fixed the ice tile recursion logic to prevent the Player from skipping over a gap.
* Resolved game-breaking issue in puzzle mode by converting singletons for puzzle components into private, individualized references.
* Temporarily adjusted tile textures and lighting for greater visibility in puzzle mode (hotfix).

### Changed
* Connected island completion status system to YarnSpinner InMemoryVariableStorage.
* Refactored the PlayerFixedMovement logic and tile type handling for a more modular approach.
* Updated some properties with more Odin Inspector features to improve the debugging options in several classes.

## [v1.0.1] - 2026-03-24

### Added
* Added Puzzle2.prefab to the Ice Island.
* Added orientation handling to the Player class so that the Player faces upwards during Puzzle mode.
* Added UI Actions to Puzzle action map on the PlayerControls.inputactions asset.
* Created the "Ground" layer mask.
* Added a death barrier/void to the Ice Island.

### Fixed
* Fixed infinite jumping bug by adding a raycast-based ground checking system.
* Fixed a graphical issue to ensure the surrounding edges of Skye's sprite within the 3D environment was transparent.
* Fixed the speed of the Player's idle animation by making it frame-independent.

### Changed
* Changed the StartArea and SnowIsland_v1 prefab object to be within the Ground layer mask.
* Restored missing code from the puzzle completion status system within the Island scripts (WIP).

## [v1.0.0] - 2026-03-12

### Added
* Added baseline ice tile "sliding" mechanic.
* Added volume sliders that work with music & SFX to fade playback in and out.
* Added neon start and end waypoints for Player in puzzle mode.
* Added dynamic tile texturing for ice tiles and normal tiles.
* Added height map to tiles for further depth.
* Added Penguin character sprite into Ice_Island.unity scene.

### Fixed
* Fixed an issue to disable the interaction prompt even while paused.

### Removed
* Removed old prototype scenes from build settings, leaving only the main menu and the Ice Island.
* Removed the splash page feature.
* Disabled the casting of shadows from tiles.

## [v0.4.3-prototype.1] - 2026-03-11

### Added
* Added animated idling sprite for Skye.
* Added basic landscape collision for the Ice Island.
* Added code for different tile types through an Enum (WIP).
* Added more controls to the Options menu.
* Added 'ESC' to the input action map for pausing.

### Fixed
* Fixed a bug where the Player was able to move the start and end tiles (this should be disabled).

### Changed
* Improved visual accessibility by adding a black outline to the interaction prompt UI pop-up.
* Restricted the interaction prompt to Exploration mode (WIP).
* Resized the Options menu to fit in the screen.

## [v0.4.2-prototype.1] - 2026-03-09

### Added
* Added the ice island island model.
* Added limited movement card usages into puzzle mode.

### Changed
* Updated the puzzle informational system to properly handle completion statuses.

## [v0.4.1-prototype.1] - 2026-03-08

### Added
* Added IslandManager.prefab & Island Scripts (WIP).

### Fixed
* Temporarily patched a bug in which the onPuzzleTrigger() wasn't switching to Puzzle state.
* Found and resolved syntactical bug regarding the playerMoved invocation in PlayerFixedMovement.cs.

### Changed
* Configured the puzzle camera & UI to be more aesthetically pleasing.
     * Also made the UI not block the actual tiles in the scene.
* Changed the puzzle camera to be disabled by default due to initialization sequencing with the GameStateManager.

## [v0.4.0-prototype.3] - 2026-03-07

### Added
* Added dialogue for Judith NPC (WIP).
* Added configuration for the end tile to trigger puzzle completion.
* Added the 3D mountain asset into the Ice_Island.unity scene.
* Added script for persistent variable storage for dialogue system.

### Fixed
* Fixed an error where the quit button in MainMenu.unity wasn't working.

### Changed
* Renamed & refactored tile-related scripts for puzzle system.
* Changed the MainMenu.unity scene to load up Ice_Island.unity instead of DialogueTest_1.unity.

## [v0.4.0-prototype.2] - 2026-03-07

### Added
* Added Mana Data to Puzzle System.
* Connected the Mana data to a counter in the puzzle UI.
* Added UI interaction prompt when the Player's raycast hits an interactable object.

### Fixed
* Fixed an error with a duplicated event system in Ice_Island.unity.
* Fixed an issue where the Player was able to move a Player-occupied tile.

## [v0.4.0-prototype.1] - 2026-03-05

### Added
* Added fixed Player movement to navigate across tiles in puzzle mode.
* Created the PhysicsManager that integrates with the current GameStateManager system.
* Added kinematics handling for the Player character.
* Created the first puzzle prefab on the Ice Island.
* Created PuzzleInformation.cs script to harbor important information about each puzzle prefab.
* Added settings menu section to the pause menu UI.
* Added Figma UI volume sliders in the settings menu (WIP).

### Changed
* Finalized the Music Manager system.
* Enhanced InteractablePillar.cs to package and send PuzzleInformation data during event firing.
* Updated puzzle trigger event handling in the GameStateManager system.
* Changed puzzle UI and camera handling to be dynamically assigned.
* Changed the default scene to MainMenu.unity.

## [v0.3.0-prototype.1] - 2026-03-02

### Added
* Added the first half of the greyboxed ice island.
* Added a functional game state manager to the ice island.

### Fixed
* Fixed an issue where the cursor was unavailable for use in the SplashPage.unity scene.

### Removed
* Removed an extra audio listener component that was causing warnings in the editor.

## [v0.2.1-prototype.1] - 2026-03-01

### Added
* Added Dialogue state to Game State Manager.
* Created a lock on the camera during dialogue sequences to allow for proper UI interaction.

### Fixed
* Fixed an issue where the Player was not able to progress through the intro dialogue sequence.

## [v0.2.0-prototype.2] - 2026-02-26

### Added
* Added a polished game loop between DialogueTest_1.unity and TileMoveExperiment.unity
* Functional Game State Manager system with 4 states: Exploration, Paused, Puzzle, Menu.
* Prefabs for Player, Game State Manager, Audio Manager, and Music Manager.
* Added intro sequence dialogue (WIP).
* Proper pause menu toggling.

### Fixed
* Fixed an issue where the movement interaction scene appeared to be frozen.
* Fixed an issue plaguing addressable assets schemas used by YarnSpinner in local builds.
* Fixed an issue where the action maps weren't switching properly.

### Changed
* Enhanced Figma UIs in the MainMenu.unity scene.

### Removed
* Removed the MovementInteraction_1.unity scene button from the splash page screen.

## [v0.2.0-prototype.1] - 2026-02-23

### Added
* Two new prototype scenes to playtest.
* UI Main Menu prototypes from Figma.
* Proof-of-concept "Bridge" experiment.
* Selectable tiles that can be highlighted when chosen.
* Grid system for puzzle mechanic.
* Mockup of a game state switch from Exploration mode to Puzzle mode.
* Early implementation of Game State Manager.
* Grey boxing Ice Island.
* Music Manager system with looping tracks.

### Changed
* TileMoveExperiment.unity upgraded from one tile to a full layout with different tiles to select and move.
     * New grid system prevents tiles from colliding with one another.
* SplashPage.unity has become the new landing page for early releases.
     * MainMenu.unity under construction.
* Enhanced Game State Manager system and scripts.

## [v0.1.0-prototype.1] - 2026-02-15

### Added
* Four prototype scenes to playtest.
* Main Menu splash page to navigate between scenes in the current stage of the project.
* 4-directional movement for puzzle prototype system.
* Basic Player movement and interaction system with raycasting in a 3D environment.
* YarnSpinner Dialogue system with successful voice over response.
* Prototyped UI SFX system.
* Created Game State Manager script (not yet implemented).
