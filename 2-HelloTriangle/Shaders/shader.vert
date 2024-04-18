
#version 430 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in int flag;

out vec3 Color;

void main(void)
{   
     if(flag==0)
          Color=vec3(1.0,0.0,0.0);
     else if(flag==1)
          Color=vec3(0.0,1.0,0.0);
     else if(flag==2)
          Color=vec3(0.0,0.0,1.0);
     else if(flag==-1)
          Color=vec3(1.0);
    gl_Position = vec4(aPosition, 1.0);
}