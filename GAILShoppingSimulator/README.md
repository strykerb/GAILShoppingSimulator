GAIL Shopping Simulator

Unity Version: 2020.3.14f1
ML Agents Version: 2.1.0-exp.1

To run Reinforcement training, navigate to the Agent_Shop scene:
1) Open the simulation Prefab
2) Set all three of the ShoppingAgents behavior parameters -> behavior type to Default
3) (Optional) Check the box labeled Stage 1 on the Shopping Agent Script; simplifies shopping task for early learning
3) Ensure as many simulations you want to run concurrently are active.
4) Modify your learning configuration file located at config\Reinforcememt\shopper.yaml
5) Activate your virtual environment
6) Run the following shell command: mlagents-learn config\Reinforcememt\shopper.yaml --run-id=######
7) Press play in the Unity Editor

To Record GAIL Demonstrations:
1) Connect a VR Headset to Unity (Oculus Quest tested)
2) Open the VR_Shop scene
3) Enter play mode 
4) Press down both triggers simultaneously and complete the shopping task


If you have further questions about the project, email Styker Buffington at sbuffing@ucsc.edu