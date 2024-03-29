locals {
  project = "wordle-arena-418422"
}

resource "google_service_account" "arena" {
  project = local.project
  account_id = "wordle-arena"
}

resource "google_service_account_iam_binding" "wordle_backend_binding" {
  service_account_id = google_service_account.arena.name
  role               = "roles/iam.workloadIdentityUser"

  members = [
    "serviceAccount:wordle-arena-418422.svc.id.goog[wordle/arena-backend]"
  ]
}