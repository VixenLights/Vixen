#pragma once

using namespace System;
#include <Box2D/Box2D.h>

namespace VixenModules
{
	namespace Effect
	{
		namespace Liquid
		{
			public enum class WrapperParticleType
			{
				Elastic,
				Powder,
				Tensile,
				Spring,
				Viscous,
				StaticPressure,
				Water,
				Reactive,
				Repulsive,
			};

			public ref class EmitterWrapper
			{
			public:
				property int X;
				property int Y;
				property System::Drawing::Color Color;
				property int Brightness;
				property int Lifetime;
				property int Velocity;
				property int SourceSize;
				property int Flow;
				property WrapperParticleType ParticleType;
				property int Angle;
			};


			public ref class LiquidFunWrapper
			{
			public:
				void Initialize(
					int width,
					int height,
					bool bottom,
					bool top,
					bool left,
					bool right,
					int particleSize,
					int lifetime);

				void FinalizeWorld();
				
				array < System::Tuple<int, int, System::Drawing::Color>^ >^ GetParticles();
			
				void StepWorld(
					float stepSeconds,
					bool mixColors,
					array<EmitterWrapper^>^ emitters);

				void CreateParticles(
					b2ParticleSystem* ps,
					int x,
					int y,
					int direction,
					int velocity,
					int flow,					
					int lifetime,
					int width,
					int height,
					const b2Color& color,
					WrapperParticleType particleType,
					bool mixcolors,					
					int sourceSize);
								
			private:

				b2ParticleFlag ConvertParticleType(WrapperParticleType particleType);
				void CreateParticleSystem(b2World* world, int lifetime, int size);
				void CreateBarrier(b2World* world, float x, float y, float width, float height);
				double rand01();

				b2World *_world;
				int _width;
				int _height;

				const int MaxColorIntensity = 255;
			};
		}
	}
}
