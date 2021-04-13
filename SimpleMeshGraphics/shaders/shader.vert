#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec2 vUv;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform vec3 viewPos = vec3(0.0,0.0,0.0);
uniform vec4 uColor = vec4(1.0,1.0,1.0,1.0);

out vec3 fNormal;
out vec3 fPos;
out vec4 fColor;

void main()
{
    //Multiplying our uniform with the vertex position, the multiplication order here does matter.
    gl_Position = uProjection * uView * uModel * vec4(vPos, 1.0);
    //We want to know the fragments position in World space, so we multiply ONLY by uModel and not uView or uProjection
    fPos = vec3(uModel * vec4(vPos, 1.0));
    //The Normal needs to be in World space too, but needs to account for Scaling of the object
    fNormal = mat3(transpose(inverse(uModel))) * vPos;
    //The colour needs to be passed through
    fColor = uColor;
}