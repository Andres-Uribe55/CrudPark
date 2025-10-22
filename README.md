# CrudPark API - Sistema de Gestión de Parqueadero

## 📋 Objetivo del Proyecto

CrudPark API es el backend de un sistema integral de gestión de parqueaderos desarrollado en **ASP.NET Core 8** con **PostgreSQL**. Esta API REST proporciona todas las funcionalidades administrativas y operativas necesarias para:

- **Gestión de Mensualidades**: Registro y control de clientes con pago mensual
- **Gestión de Operadores**: Administración de usuarios que operan el sistema
- **Configuración de Tarifas**: Definición de reglas de cobro por tipo de vehículo
- **Control de Entradas/Salidas**: Registro de tickets para vehículos invitados
- **Sistema de Pagos**: Cálculo automático y registro de cobros
- **Autenticación JWT**: Seguridad basada en tokens para proteger endpoints

El sistema está diseñado para integrarse con:
- **Frontend Vue.js**: Panel administrativo web
- **Aplicación Java Desktop**: Sistema operativo para cabina de entrada/salida

Ambos sistemas comparten la misma base de datos PostgreSQL para comunicarse en tiempo real.

---

## 🛠️ Tecnologías Utilizadas

- **ASP.NET Core 8** (Web API)
- **Entity Framework Core 8** (ORM)
- **PostgreSQL** (Base de datos)
- **JWT (JSON Web Tokens)** (Autenticación)
- **BCrypt** (Hashing de contraseñas)
- **AutoMapper** (Mapeo de DTOs)
- **Swagger/OpenAPI** (Documentación de API)

---

## 🚀 Instrucciones de Instalación y Ejecución

### **Requisitos Previos**

1. **.NET 8 SDK** instalado
```bash
   dotnet --version
   # Debe mostrar 8.0.x
```

2. **PostgreSQL** instalado y en ejecución
   - Puerto por defecto: `5432`
   - Usuario: `postgres`

3. **IDE recomendado**: Rider, Visual Studio 2022, o Visual Studio Code

---

### **Paso 1: Clonar o descargar el proyecto**
```bash
git clone <url-del-repositorio>
cd CrudPark.API
```

---

### **Paso 2: Configurar la cadena de conexión**

Abre el archivo `appsettings.json` y actualiza la cadena de conexión con tus credenciales de PostgreSQL:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=crudpark_db;Username=postgres;Password=TU_PASSWORD_AQUI"
  }
}
```

---

### **Paso 3: Restaurar dependencias**
```bash
dotnet restore
```

---

### **Paso 4: Aplicar migraciones (crear tablas en la BD)**
```bash
dotnet ef database update
```

Este comando creará automáticamente todas las tablas necesarias:
- `memberships` (Mensualidades)
- `operators` (Operadores)
- `rates` (Tarifas)
- `tickets` (Entradas/Salidas)
- `payments` (Pagos)

---

### **Paso 5: Ejecutar la aplicación**
```bash
dotnet run
```

O desde tu IDE, presiona el botón **Run/Debug**.

La aplicación se ejecutará en:
- **HTTPS**: `https://localhost:7XXX`
- **HTTP**: `http://localhost:5XXX`

---

### **Paso 6: Acceder a Swagger**

Abre tu navegador y navega a:
```
https://localhost:7XXX/swagger
```

Aquí podrás explorar y probar todos los endpoints de la API de forma interactiva.

---

## 🗄️ Configuración de la Base de Datos (PostgreSQL)

### **Estructura de Tablas**

#### **1. Memberships (Mensualidades)**
```sql
- id (int, PK)
- client_name (varchar)
- email (varchar)
- phone (varchar)
- license_plate (varchar, unique)
- vehicle_type (int) -- 0:Bicycle, 1:Motorcycle, 2:Car, 3:Truck
- start_date (timestamp)
- end_date (timestamp)
- is_active (boolean)
- created_at (timestamp)
- updated_at (timestamp, nullable)
```

#### **2. Operators (Operadores)**
```sql
- id (int, PK)
- full_name (varchar)
- email (varchar, nullable)
- username (varchar, unique)
- password (varchar) -- Hash BCrypt
- is_active (boolean)
- created_at (timestamp)
- updated_at (timestamp, nullable)
```

#### **3. Rates (Tarifas)**
```sql
- id (int, PK)
- rate_name (varchar)
- vehicle_type (int)
- hourly_rate (decimal)
- fraction_rate (decimal)
- daily_cap (decimal)
- grace_period_minutes (int) -- Default: 30
- is_active (boolean)
- created_at (timestamp)
- updated_at (timestamp, nullable)
```

#### **4. Tickets (Entradas/Salidas)**
```sql
- id (int, PK)
- folio (varchar, unique)
- license_plate (varchar)
- vehicle_type (int)
- entry_date_time (timestamp)
- exit_date_time (timestamp, nullable)
- entry_type (int) -- 0:Membership, 1:Guest
- entry_operator_id (int, FK)
- exit_operator_id (int, FK, nullable)
- total_minutes (int, nullable)
- membership_id (int, FK, nullable)
- qr_code (varchar)
```

#### **5. Payments (Pagos)**
```sql
- id (int, PK)
- ticket_id (int, FK)
- amount_charged (decimal)
- payment_method (int) -- 0:Cash, 1:Card, 2:Transfer
- payment_date_time (timestamp)
- operator_id (int, FK)
```

---

### **Relaciones entre Tablas**

- `Ticket` → `Operator` (entrada y salida)
- `Ticket` → `Membership` (si es cliente mensual)
- `Payment` → `Ticket` (un pago por ticket)
- `Payment` → `Operator` (quién registró el pago)
- `Rate` → `VehicleType` (una tarifa por tipo de vehículo)

---

### **Verificar la Base de Datos**

Usa **DBeaver** o **pgAdmin** para conectarte a PostgreSQL y verificar:
```sql
-- Ver todas las tablas creadas
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public';

-- Verificar estructura de una tabla
\d memberships
```

---

## 🔄 Flujo General de Uso

### **1. Configuración Inicial (Administrador Web)**

#### **a) Crear Operadores**
```http
POST /api/operators
```
El administrador crea usuarios operadores que trabajarán en las cabinas.

#### **b) Configurar Tarifas**
```http
POST /api/rates
```
Define las tarifas para cada tipo de vehículo:
- Valor por hora
- Valor por fracción
- Tope diario
- Tiempo de gracia (ej: 30 minutos gratis)

#### **c) Registrar Mensualidades**
```http
POST /api/memberships
```
Registra clientes con pago mensual (nombre, placa, fechas de vigencia).

---

### **2. Operación Diaria (Aplicación Java/Cabina)**

#### **a) Login de Operador**
```http
POST /api/operators/authenticate
Body: { "username": "operador1", "password": "password123" }
Response: { "token": "eyJhbGc...", ... }
```
El operador inicia sesión y obtiene un **token JWT** válido por 2 horas.

#### **b) Entrada de Vehículo**
```http
POST /api/tickets/entry
Headers: Authorization: Bearer {token}
Body: {
  "licensePlate": "ABC123",
  "vehicleType": 2,
  "entryOperatorId": 1
}
```

**El sistema automáticamente:**
1. Verifica si la placa tiene mensualidad vigente
2. Si SÍ → marca como `EntryType.Membership` (sin cobro)
3. Si NO → marca como `EntryType.Guest` (se cobrará al salir)
4. Genera un `folio` único
5. Crea un código QR para el ticket

#### **c) Salida de Vehículo**
```http
POST /api/tickets/exit
Headers: Authorization: Bearer {token}
Body: {
  "identifier": "ABC123", // Puede ser placa o folio
  "exitOperatorId": 1
}
```

**El sistema automáticamente:**
1. Busca el ticket activo
2. Registra la hora de salida
3. Calcula el tiempo total de estadía
4. Si es mensualidad vigente → permite salida sin cobro
5. Si es invitado → calcula el monto según la tarifa

#### **d) Calcular Monto a Cobrar**
```http
GET /api/payments/calculate/{ticketId}
Headers: Authorization: Bearer {token}
```

**Lógica de cálculo:**
```
1. Tiempo total = Salida - Entrada (en minutos)
2. Si tiempo <= 30 min (gracia) → $0
3. Si tiempo > 30 min:
   - Minutos cobrables = total - 30
   - Horas completas * tarifa_hora
   - Fracción restante * tarifa_fracción
   - Aplicar tope diario si existe
```

#### **e) Registrar el Pago**
```http
POST /api/payments
Headers: Authorization: Bearer {token}
Body: {
  "ticketId": 123,
  "amountCharged": 5000.00,
  "paymentMethod": 0, // Cash
  "operatorId": 1
}
```

---

### **3. Monitoreo y Reportes (Administrador Web)**

#### **a) Dashboard de Métricas**
```http
GET /api/payments/daily-revenue?date=2024-10-21
GET /api/tickets (filtrar por activos)
GET /api/memberships/expiring?days=3
```

#### **b) Reportes Financieros**
```http
GET /api/payments/revenue-range?startDate=2024-10-01&endDate=2024-10-31
GET /api/payments/by-operator/{operatorId}
```

#### **c) Gestión de Mensualidades**
```http
GET /api/memberships/search?plate=ABC123
PATCH /api/memberships/{id}/status (activar/desactivar)
PUT /api/memberships/{id} (actualizar datos)
```

---

## 🔐 Autenticación y Seguridad

### **Uso del Token JWT**

1. **Obtener token:**
```http
   POST /api/operators/authenticate
```

2. **Usar token en peticiones:**
```http
   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

3. **Endpoints públicos (sin token):**
   - `POST /api/operators/authenticate`

4. **Endpoints protegidos (requieren token):**
   - Todos los demás endpoints de la API

---

### **Probar Autenticación en Swagger**

1. Ir a `POST /api/operators/authenticate`
2. Ejecutar con credenciales válidas
3. Copiar el `token` de la respuesta
4. Hacer clic en el botón **"Authorize" (🔒)** en la parte superior
5. Ingresar: `Bearer {tu_token_aquí}`
6. Ahora todos los endpoints protegidos funcionarán

---

## 📊 Arquitectura del Proyecto
```
CrudPark.API/
├── Controllers/          # Endpoints HTTP (capa de presentación)
│   ├── MembershipsController.cs
│   ├── OperatorsController.cs
│   ├── RatesController.cs
│   ├── TicketsController.cs
│   └── PaymentsController.cs
│
├── Services/            # Lógica de negocio
│   ├── MembershipService.cs
│   ├── OperatorService.cs
│   ├── RateService.cs
│   ├── TicketService.cs
│   ├── PaymentService.cs
│   └── AuthService.cs (Generación de JWT)
│
├── Repositories/        # Acceso a datos (SQL)
│   ├── MembershipRepository.cs
│   ├── OperatorRepository.cs
│   ├── RateRepository.cs
│   ├── TicketRepository.cs
│   └── PaymentRepository.cs
│
├── Models/              # Entidades (tablas de BD)
│   ├── Membership.cs
│   ├── Operator.cs
│   ├── Rate.cs
│   ├── Ticket.cs
│   ├── Payment.cs
│   └── Enums (VehicleType, PaymentMethod, EntryType)
│
├── DTOs/                # Data Transfer Objects
│   ├── OperatorCreateDto.cs
│   ├── OperatorResponseDto.cs
│   ├── AuthResponseDto.cs
│   └── LoginDto.cs
│
├── Data/                # Contexto de base de datos
│   └── AppDbContext.cs
│
├── Configuration/       # Configuraciones auxiliares
│   └── MappingProfile.cs (AutoMapper)
│
└── Program.cs           # Configuración principal
```

---

## 🧪 Endpoints Disponibles

### **Memberships (Mensualidades)**
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/memberships` | Listar todas |
| GET | `/api/memberships/{id}` | Obtener por ID |
| GET | `/api/memberships/search?plate={plate}` | Buscar por placa |
| POST | `/api/memberships` | Crear nueva |
| PUT | `/api/memberships/{id}` | Actualizar |
| PATCH | `/api/memberships/{id}/status` | Cambiar estado |
| GET | `/api/memberships/expiring?days=3` | Próximas a vencer |

---

### **Operators (Operadores)**
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/operators` | Listar todos |
| GET | `/api/operators/{id}` | Obtener por ID |
| POST | `/api/operators` | Crear nuevo |
| PUT | `/api/operators/{id}` | Actualizar |
| PUT | `/api/operators/{id}/password` | Cambiar contraseña |
| POST | `/api/operators/authenticate` | Login (JWT) |

---

### **Rates (Tarifas)**
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/rates` | Listar todas |
| GET | `/api/rates/{id}` | Obtener por ID |
| POST | `/api/rates` | Crear nueva |
| DELETE | `/api/rates/{id}` | Eliminar |

---

### **Tickets (Entradas/Salidas)**
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/tickets/entry` | Registrar entrada |
| POST | `/api/tickets/exit` | Registrar salida |
| GET | `/api/tickets/{identifier}` | Consultar ticket |

---

### **Payments (Pagos)**
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/payments` | Listar todos |
| GET | `/api/payments/{id}` | Obtener por ID |
| GET | `/api/payments/by-ticket/{ticketId}` | Pagos de un ticket |
| GET | `/api/payments/by-operator/{operatorId}` | Pagos de un operador |
| POST | `/api/payments` | Registrar pago |
| GET | `/api/payments/calculate/{ticketId}` | Calcular monto |
| GET | `/api/payments/daily-revenue?date={date}` | Ingresos del día |
| GET | `/api/payments/revenue-range?start={}&end={}` | Ingresos por rango |

---

## 🔧 Comandos Útiles
```bash
# Restaurar dependencias
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar la aplicación
dotnet run

# Crear una nueva migración
dotnet ef migrations add NombreDeLaMigracion

# Aplicar migraciones pendientes
dotnet ef database update

# Revertir a una migración específica
dotnet ef database update NombreDeLaMigracion

# Eliminar la última migración (si no se aplicó)
dotnet ef migrations remove

# Generar script SQL de las migraciones
dotnet ef migrations script
```

---

## 📝 Notas Importantes

### **Seguridad**
- ⚠️ La clave JWT está hardcodeada en el código. En producción, muévela a `appsettings.json` o variables de entorno.
- ⚠️ Las contraseñas se almacenan con hash BCrypt (nunca en texto plano).
- ⚠️ Los tokens JWT expiran en 2 horas.

### **CORS**
- Configurado para aceptar peticiones desde:
  - `http://localhost:5173` (Vue.js en desarrollo)
  - `http://localhost:3000` (Vue.js alternativo)

### **Validaciones**
- No se permite crear mensualidades con placas duplicadas si existe una vigente
- No se permite registrar salida sin entrada previa
- No se puede pagar un ticket que ya tiene pago registrado
- Todos los cálculos respetan el tiempo de gracia (30 minutos por defecto)

---

## 👥 Roles y Permisos

Actualmente el sistema tiene un rol único:
- **Operator**: Puede acceder a todos los endpoints protegidos

*Nota: El sistema está preparado para expandirse a múltiples roles (Admin, Operator, Viewer) en futuras versiones.*

---

## 🐛 Solución de Problemas

### **Error: "Cannot connect to PostgreSQL"**
- Verifica que PostgreSQL esté corriendo: `pg_isready`
- Verifica las credenciales en `appsettings.json`
- Verifica el puerto (por defecto 5432)

### **Error: "Relation does not exist"**
- Ejecuta las migraciones: `dotnet ef database update`

### **Error: "401 Unauthorized" en todos los endpoints**
- Verifica que el token JWT esté en el header: `Authorization: Bearer {token}`
- Verifica que el token no haya expirado (2 horas de validez)
- Genera un nuevo token con `/api/operators/authenticate`

### **Error: "Timestamp with/without time zone"**
- Este error fue resuelto configurando todas las columnas DateTime como `timestamp without time zone`
- Si persiste, verifica que la migración `FixDateTimeTypes` esté aplicada

---

## 📞 Contacto y Soporte

Para dudas o problemas con la API, contactar al equipo de desarrollo.

---

## 📄 Licencia

Este proyecto es parte de un ejercicio académico/profesional de desarrollo de software.

---

**¡El sistema CrudPark API está listo para integrarse con el frontend Vue.js y la aplicación Java Desktop!** 🚀
