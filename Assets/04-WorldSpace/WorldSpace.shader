Shader "Custom/WorldSpace" {
	//David Brookwell-Reuber
	//1650478
	//Shader Lab 1
	properties{
		_SeaLevel ("Sea Level", Float) = 0.0
		_TreeLevel ("Tree Line", Float) = 2.0
		_SnowCap ("Icecap", Float) = 3.0
	}
	
	SubShader {
      Pass {
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         // uniform float4x4 _Object2World; 
            // automatic definition of a Unity-specific uniform parameter
 		 float _SeaLevel;
 		 float _TreeLevel;
 		 float _SnowCap;
 		 
         struct vertexInput {
            float4 vertex : POSITION;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 position_in_world_space : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output; 
 
            output.pos =  mul(UNITY_MATRIX_MVP, input.vertex);
            output.position_in_world_space = 
               mul(_Object2World, input.vertex);
               // transformation of input.vertex from object 
               // coordinates to world coordinates;
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR 
         {
         	if(input.position_in_world_space.y < _SeaLevel){
         		return float4(0,0,1,1);
         	}
         	if(input.position_in_world_space.y < _TreeLevel){
         		return float4(0,1,0,1);
         	}
         	if(input.position_in_world_space.y > _SnowCap){
         		return float4(1,1,1,1);
         	}
            return float4(0.5,0.5,0.5,0.5);
          }
 
         ENDCG  
      }
   }
}