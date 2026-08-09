import { createFileRoute, Outlet } from "@tanstack/react-router";

export const Route = createFileRoute("/obras/$id")({
  component: () => <Outlet />,
});
