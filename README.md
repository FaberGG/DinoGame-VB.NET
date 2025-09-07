# Juego Dinosaurio - Demo Educativa VB.NET

## 📝 Descripción del Proyecto

Este proyecto es una implementación educativa del clásico **Chrome Dino Game** desarrollado completamente en **Visual Basic .NET (VB.NET)** usando **Windows Forms**. El juego presenta un dinosaurio (representado como un cuadrado azul) que debe saltar sobre obstáculos rojos para obtener la mayor puntuación posible.
<img width="995" height="554" alt="image" src="https://github.com/user-attachments/assets/294f31b8-97ca-4af7-b227-0484169020dc" />

## 🎯 Propósito Educativo

El objetivo principal de este proyecto es servir como **demostración práctica de las características fundamentales del lenguaje VB.NET**, incluyendo:

### Conceptos de Programación Demostrados

- **Programación Orientada a Objetos (POO)**:
  - Clases y encapsulamiento (`Player`, `Obstacle`, `ObstacleManager`)
  - Métodos y propiedades

- **Manejo de Eventos**:
  - Eventos de teclado (`KeyDown`)
  - Eventos de timer (`Timer.Tick`)
  - Eventos de pintado (`Paint`)

- **Estructuras de Control**:
  - Condicionales (`If...Then...Else`)
  - Bucles (`For Each`)
  - Estructuras `Select Case`

- **Tipos de Datos y Variables**:
  - Tipos primitivos (`Integer`, `Boolean`, `Single`)
  - Colecciones (`List(Of T)`)
  - Constantes (`Const`)

- **Gráficos y Interfaz de Usuario**:
  - Windows Forms
  - Renderizado con `Graphics`
  - Manejo de colores y fuentes

- **Gestión de Memoria**:
  - Uso de `Using` para liberación automática de recursos
  - Optimizaciones de rendimiento

## 🚀 Características del Juego

- **Física**: Sistema de gravedad y salto
- **Dificultad Progresiva**: La velocidad aumenta gradualmente con la puntuación
- **Detección de Colisiones**: Sistema preciso usando intersección de rectángulos
- **Optimización de Rendimiento**: Renderizado a 50 FPS con doble buffer
- **Interfaz Intuitiva**: Controles simples y retroalimentación visual clara

## 🎮 Controles

| Tecla | Función |
|-------|---------|
| `Espacio` / `↑` | Saltar (durante el juego) / Reiniciar (en Game Over) |
| `Esc` | Salir del juego |

## 🏗️ Arquitectura del Código

El proyecto está estructurado en **4 clases principales**:

1. **`DinoGameForm`** - Formulario principal que coordina el juego
2. **`Player`** - Representa al jugador con mecánicas de salto
3. **`Obstacle`** - Obstáculos individuales que se mueven por pantalla
4. **`ObstacleManager`** - Gestor que administra múltiples obstáculos


## 📚 Conceptos de VB.NET Enseñados

### Sintaxis y Características del Lenguaje
- Declaración de variables con `Dim`, `Private`, `Public`
- Uso de `Imports` para namespaces
- Regiones de código (`#Region...#End Region`)
- Comentarios de documentación
- Manejo de tipos `Single` vs `Double` para optimización

### Programación Orientada a Objetos
- Constructores (`Public Sub New()`)
- Encapsulamiento con modificadores de acceso
- Métodos y funciones (`Sub` vs `Function`)
- Parámetros y valores de retorno

### Windows Forms y Gráficos
- Herencia de controles Windows Forms
- Manejo de eventos (`Handles`, `AddHandler`)
- Renderizado personalizado con `Graphics`
- Optimizaciones de rendimiento (`DoubleBuffer`)

### Buenas Prácticas
- Código modular y reutilizable
- Separación de responsabilidades
- Nomenclatura consistente
- Documentación 
- Gestión eficiente de recursos

## 🎓 Valor Educativo

Este proyecto es ideal para:

- **Estudiantes** aprendiendo VB.NET
- **Profesores** que necesitan ejemplos prácticos
- **Desarrolladores** migrando de otros lenguajes
- **Programadores** que buscan refrescar conceptos de POO

## 📋 Requisitos del Sistema

- **Framework**: .NET Framework 4.5 o superior
- **IDE**: Visual Studio 2015 o superior
- **OS**: Windows 7 SP1 o superior

## 🚀 Cómo Ejecutar

1. Clona o descarga el proyecto
2. Abre la solución en Visual Studio
3. Compila el proyecto (F6)
4. Ejecuta la aplicación (F5)

---

> **Nota**: Este proyecto está diseñado con fines educativos. El código incluye comentarios extensivos para facilitar el aprendizaje y comprensión de los conceptos de VB.NET.
