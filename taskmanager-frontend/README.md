# TaskManager Frontend

React-based frontend for the TaskManager API.

## Features

- 🔐 JWT Authentication (Login & Register)
- ✅ Task Management (CRUD operations)
- 📊 Task Statistics Dashboard
- 🎨 Modern UI with Tailwind CSS
- 📱 Responsive Design
- ⚡ Built with React + TypeScript + Vite

## Getting Started

### Prerequisites

- Node.js 16+ and npm/yarn
- Backend running at `https://localhost:5001`

### Installation

```bash
cd taskmanager-frontend
npm install
```

### Development

```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`

### Testing

```bash
npm test              # Run all tests
npm test:ui          # Run with UI
npm test:coverage    # Generate coverage report
```

See [TESTING.md](./TESTING.md) for detailed testing guide.

### Build

```bash
npm run build
```

### Environment Variables

Create a `.env` file in the root directory:

```env
VITE_API_URL=https://localhost:5001/api
```

## Project Structure

```
src/
├── components/         # React components
│   ├── ProtectedRoute.tsx
│   ├── TaskForm.tsx
│   ├── TaskList.tsx
│   └── TaskEditModal.tsx
├── pages/              # Page components
│   ├── Login.tsx
│   ├── Register.tsx
│   └── Dashboard.tsx
├── services/           # API client
│   └── api.ts
├── store/              # Zustand stores
│   ├── authStore.ts
│   └── taskStore.ts
├── types/              # TypeScript types
│   └── index.ts
├── App.tsx             # Main app component
├── main.tsx            # Entry point
└── index.css           # Global styles
```

## Technologies

- **React 18** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool
- **Tailwind CSS** - Styling
- **React Router** - Navigation
- **Zustand** - State management
- **Axios** - HTTP client
- **Lucide React** - Icons
