#version 330 core

in vec2 texCoord;
in float texindex;

uniform sampler2D t[128];

out vec4 outputColor;

void main()
{  
	outputColor = texture(t[int(texindex)], texCoord);  
}