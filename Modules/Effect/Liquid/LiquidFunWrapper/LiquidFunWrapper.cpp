#include "stdafx.h"
#include "LiquidFunWrapper.h"



void VixenModules::Effect::Liquid::LiquidFunWrapper::Initialize(
	int width,
	int height,
	bool bottom,
	bool top,
	bool left,
	bool right,
	int particleSize,
	int lifetime)	
{	
	_width = width;
	_height = height;
	
	b2Vec2 gravity;
	gravity.Set(0.0f, -10.0f);
	_world = new b2World(gravity);

	CreateParticleSystem(_world, lifetime, particleSize);

	if (bottom)
	{
		CreateBarrier(_world, (float)_width / 2.0, -1.0f, (float)_width, 0.001f); 
	}
	if (top)
	{
		CreateBarrier(_world, (float)_width / 2.0, _height + 1.0f, (float)_width, 0.001f);
	}
	if (left)
	{
		CreateBarrier(_world, -1.0f, (float)_height / 2.0f, 0.001f, (float)_height);
	}
	if (right)
	{
		CreateBarrier(_world, (float)_width + 1.0f, (float)_height / 2.0f, 0.001f, (float)_height);
	}							
}

array < System::Tuple<int, int, System::Drawing::Color>^ >^ VixenModules::Effect::Liquid::LiquidFunWrapper::GetParticles()
{	
	// Get the particle systems in the world
	b2ParticleSystem* particleSystems = _world->GetParticleSystemList();

	// Get the particle position array
	const b2Vec2* positionBuffer = particleSystems->GetPositionBuffer();
	
	// Get the particle color array
	const b2ParticleColor* colorBuffer = particleSystems->GetColorBuffer();

	array<Tuple<int, int, System::Drawing::Color>^ >^ particles = gcnew array<Tuple<int, int, System::Drawing::Color>^>(particleSystems->GetParticleCount()); 
	
	// Loop over the particle systems in the world
	for (b2ParticleSystem* p = particleSystems; p; p = p->GetNext())
	{		
		// Loop over the particles in the particle system
		for (int i = 0; i < p->GetParticleCount(); ++i)
		{
			// Get the position of the particle
			int x = Math::Round(positionBuffer[i].x, 0);
			int y = Math::Round(positionBuffer[i].y, 0);

			// Get the color of the particle
			auto c = colorBuffer[i].GetColor();

			// Convert the particle to a .NET type
			particles[i] = gcnew Tuple<int, int, System::Drawing::Color>(
				x, 
				y, 
				System::Drawing::Color::FromArgb((c.r * MaxColorIntensity), (c.g * MaxColorIntensity), (c.b * MaxColorIntensity)));
		}
	}

	return particles;
}


void VixenModules::Effect::Liquid::LiquidFunWrapper::LiquidFunWrapper::StepWorld(
	float stepSeconds,
	bool mixColors,
	array<EmitterWrapper^>^ emitters)
{	
	// Advance the world
	_world->Step(
		stepSeconds,	// Time
		6,					// Velocity Iterations
		2,					// Position Iterations
		3);				// Particle Iterations

	// Get the particle system(s)
	b2ParticleSystem* ps =_world->GetParticleSystemList();

	// Loop over the emitters	
	for each(EmitterWrapper^ emitter in emitters)
	{
		// Create the emitter color
		b2Color emitterColor(
			emitter->Color.R / (float)MaxColorIntensity, 
			emitter->Color.G / (float)MaxColorIntensity, 
			emitter->Color.B / (float)MaxColorIntensity);

		// Create the emitter particles
		CreateParticles(
			ps, 
			emitter->X, 
			emitter->Y, 
			emitter->Angle, 
			emitter->Velocity, 
			emitter->Flow, 			
			emitter->Lifetime, 
			_width, 
			_height, 
			emitterColor,
			emitter->ParticleType, 
			mixColors, 			
			emitter->SourceSize);
	}
		
	// Get the positions of the particles in the system
	const b2Vec2* positionBuffer = ps->GetPositionBuffer();
	
	// Get the number of particles in the system
	int particleCount = ps->GetParticleCount();
	
	// Loop over the particles 
	for (int i = 0; i < particleCount; ++i)
	{
		// Get the position of the particle
		int x = positionBuffer[i].x;
		int y = positionBuffer[i].y;

		// If the particle is outside the viewable area then...
		if (y < -1 || x < -1 || x > _width + 1)
		{
			// Destroy the particle
			ps->DestroyParticle(i);
		}
	}
}

void VixenModules::Effect::Liquid::LiquidFunWrapper::FinalizeWorld()
{
	// Get the particle system(s)
	b2ParticleSystem* ps = _world->GetParticleSystemList();

	// Get the number of particles in the system
	int particleCount = ps->GetParticleCount();

	// Loop over the particles 
	for (int i = 0; i < particleCount; ++i)
	{
		ps->DestroyParticle(i);
	}

	delete _world;
}

double VixenModules::Effect::Liquid::LiquidFunWrapper::rand01()
{
	return (double)rand() / (double)RAND_MAX;
}

b2ParticleFlag VixenModules::Effect::Liquid::LiquidFunWrapper::ConvertParticleType(WrapperParticleType particleType)
{
	b2ParticleFlag particleFlag;

	if (particleType == WrapperParticleType::Elastic)
	{
		particleFlag = b2_elasticParticle;
	}
	else if (particleType == WrapperParticleType::Powder)
	{
		particleFlag = b2_powderParticle;
	}
	else if (particleType == WrapperParticleType::Tensile)
	{
		particleFlag = b2_tensileParticle;
	}
	else if (particleType == WrapperParticleType::Spring)
	{
		particleFlag = b2_springParticle;
	}
	else if (particleType == WrapperParticleType::Viscous)
	{
		particleFlag = b2_viscousParticle;
	}
	else if (particleType == WrapperParticleType::StaticPressure)
	{
		particleFlag = b2_staticPressureParticle;
	}
	else if (particleType == WrapperParticleType::Water)
	{
		particleFlag = b2_waterParticle;
	}
	else if (particleType == WrapperParticleType::Reactive)
	{
		particleFlag = b2_reactiveParticle;
	}
	else if (particleType == WrapperParticleType::Repulsive)
	{
		particleFlag = b2_repulsiveParticle;
	}

	return particleFlag;
}

void VixenModules::Effect::Liquid::LiquidFunWrapper::CreateParticles(
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
	int sourceSize)
{	
	// Initialize the position of the particle
	float positionX = (float)x; 
	float positionY = (float)y; 

	static const float TWO_PI = 2 * Math::PI;

	float velocityX = (float)velocity * 10.0 * cos(TWO_PI * (float)direction / 360.0);
	float velocityY = (float)velocity * 10.0 * sin(TWO_PI * (float)direction / 360.0);

	float velocityVariation = rand01() * 0.1;
	velocityVariation -= velocityVariation / 2.0;

	velocityX -= velocityX * velocityVariation;
	velocityY -= velocityY * velocityVariation;

	float particleLifetime = lifetime / 100.0;
	
	// Create particles based on emitter flow
	for (int i = 0; i < flow; i++)
	{
		b2ParticleDef pd;
		pd.flags = ConvertParticleType(particleType);
		
		if (mixcolors)
		{
			pd.flags |= b2_colorMixingParticle;
		}

		pd.color.Set(color); 

		if (sourceSize == 0)
		{
			// Randomly pick a position within the emitter's radius.
			const float32 angle = rand01() * 2.0f * b2_pi;

			// Distance from the center of the circle.
			const float32 distance = rand01();
			b2Vec2 positionOnUnitCircle(sin(angle), cos(angle));

			// Initial position.
			pd.position.Set(
				positionX + positionOnUnitCircle.x * distance * 0.5,
				positionY + positionOnUnitCircle.y * distance * 0.5);
		}
		else
		{
			// Distance from the center of the circle.
			const float32 distance = rand01() * ((float)sourceSize - (float)sourceSize / 2.0);

			float offsetX = distance * cos(TWO_PI * ((float)direction + 90.0) / 360.0);
			float offsetY = distance * sin(TWO_PI * ((float)direction + 90.0) / 360.0);

			// Initial position.
			pd.position.Set(positionX + (offsetX * (float)width / 200.0), positionY + (offsetY * (float)height / 200.0));
		}

		// Send particle flying
		pd.velocity.x = velocityX;
		pd.velocity.y = velocityY;

		// Give particle a lifetime
		if (lifetime > 0)
		{
			float randomlifeTime = particleLifetime + (particleLifetime * 0.2 * rand01()) - (particleLifetime *.01);			
			pd.lifetime = randomlifeTime;
		}
		
		// If the lifetime is too small the Liquid Fun API seems to revert to a very large lifetime.
		if (pd.lifetime < 0.02)
		{
			pd.lifetime = 0.02;
		}

		ps->CreateParticle(pd);
	}	
}

void VixenModules::Effect::Liquid::LiquidFunWrapper::CreateParticleSystem(b2World* world, int lifetime, int size)
{
	b2ParticleSystemDef particleSystemDef;
	auto particleSystem = world->CreateParticleSystem(&particleSystemDef);
	particleSystem->SetRadius((float)size / 1000.0f);
	particleSystem->SetMaxParticleCount(100000);	
	particleSystem->SetDestructionByAge(true);	
}

void VixenModules::Effect::Liquid::LiquidFunWrapper::CreateBarrier(b2World* world, float x, float y, float width, float height)
{
	b2BodyDef groundBodyDef;
	groundBodyDef.position.Set(x, y);
	
	// Create the rigid body in the world
	b2Body* groundBody = world->CreateBody(&groundBodyDef);
	
	// Set the bodies width and height
	b2PolygonShape groundBox;
	groundBox.SetAsBox(width / 2.0f, height / 2.0f);

	// Associate a fixture to the body
	groundBody->CreateFixture((b2Shape*)&groundBox, 0.0f);
}



